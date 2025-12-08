using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Stage;

/// <summary>
/// スレッド対応・高速版 A* パスファインダー
/// - マップ: int[,]（0 = 通行可, 1 = 壁）
/// - SearchPathAsync: Vector3 → List<Vector3> を返す非同期API
/// </summary>
/// 

//Q : そもそもA*ってなんぞや
//A:今いるマスから上下左右（８方向）を見て進んだ距離とそのマスからゴールまでの距離を足したときの値がもっとも小さいマスに進む、以下それを繰り返す
public class AStarPathFinder
{
    // ===== フィールド =====

    private readonly int[,] map;
    private readonly int width;//マップの横幅
    private readonly int height;//マップの縦幅
    private readonly Vector3 defaultPos;   // マップ左下のワールド座標

    // 4方向
    private static readonly Vector2Int[] dirs =
    {
        new Vector2Int( 1,  0),
        new Vector2Int(-1,  0),
        new Vector2Int( 0,  1),
        new Vector2Int( 0, -1)
    };

    // ===== コンストラクタ =====

    public AStarPathFinder(int[,] map, Vector3 defaultPosition)
    {
        this.map = map;
        this.width = map.GetLength(0);
        this.height = map.GetLength(1);
        this.defaultPos = defaultPosition;
    }

    // ===== 公開API：非同期でパス探索（ワールド座標版） =====

    /// <summary>
    /// ワールド座標での start / goal からパスを非同期で計算する
    /// - 重い計算は別スレッド
    /// - 最後のワールド変換だけメインスレッドで実行
    /// </summary>
    public async UniTask<List<Vector3>> SearchPathAsync(
        Vector3 worldStart,
        Vector3 worldGoal,
        CancellationToken token = default)
    {
        // ① ワールド → マップ座標（メインスレッドでOK）
        //経路探索はデフォルトポジを無視した値での探索になるので現在地（デフォルトポジション補正ありき）を一旦マップ上単体での値に戻す
        Vector2Int startMap = WorldToMap(worldStart);
        Vector2Int goalMap = WorldToMap(worldGoal);

        // ② 重い A* 計算は別スレッドで実行（Unity API 禁止）
        //これらの計算 * ゴーストの数をCPUのUnityのパラメーター使用可能なメインスレッドでやると重いので値の計算は別スレッドでメインを邪魔しない
        List<Vector2Int> mapPath = await UniTask.RunOnThreadPool(() =>
        {
            return SearchOnGrid(startMap, goalMap, token);
        }, cancellationToken: token);

        if (mapPath == null || mapPath.Count == 0)
        {
            return null;
        }

        // ③ メインスレッドに戻ってから MapToWorld（Terrain 触ってOK）
        //defaultPosありきの値に戻す
        var worldPath = new List<Vector3>(mapPath.Count);
        foreach (var p in mapPath)
        {
            var mapInfo = new MapPositionInfo(p.x,p.y);
            worldPath.Add(StageMethods.GetTargetNodePos(mapInfo));
        }

        return worldPath;
    }

    // ===== ワールド座標 ←→ マップ配列座標（メインスレッド用） =====

    // ワールド座標 → マップ配列座標
    Vector2Int WorldToMap(Vector3 world)
    {
        // RoundToInt だと境界でズレやすいので Floor 推奨
        int x = Mathf.FloorToInt(world.x - defaultPos.x);
        int z = Mathf.FloorToInt(world.z - defaultPos.z);
        return new Vector2Int(x, z);
    }

    // ===== ここから下は「純計算の A*」部分（Unity API 一切禁止） =====

    /// <summary>
    /// グリッド上でのA*（start / goal はマップ座標）
    /// - 別スレッドで動かしてOK
    /// - Unity API を絶対に呼ばないこと
    /// </summary>
    List<Vector2Int> SearchOnGrid(Vector2Int start, Vector2Int goal, CancellationToken token)
    {
        // 範囲外・壁チェック
        //現在地と目的地がマップの範囲外だったら、よはイレギュラーなことが起きてたら探索しない
        if (!InRange(start.x, start.y) || !InRange(goal.x, goal.y))
            return null;
        //これも同じ、目的地が壁だったり現在地が壁だったとき
        if (IsWall(start.x, start.y) || IsWall(goal.x, goal.y))
            return null;

        // gCost: スタートからの実コスト
        var gCost = new int[width, height];
        // close: 探索完了フラグ、もうに二度と見ないマス（= true）を格納する
        var closed = new bool[width, height];
        // parent: どこから来たか（経路復元用）
        var parent = new Vector2Int[width, height];

        // 初期化（int は 0 初期化なので MaxValue で埋める）
        const int INF = int.MaxValue;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gCost[x, z] = INF;
                // parent[x, z] は (0,0) 初期化だが、使う時に上書きするのでOK
            }
        }

        //今後調べる可能性のあるマス
        var open = new MinHeap(64);

        // スタートノード登録
        gCost[start.x, start.y] = 0;

        //スタート地点での推定コスト
        int h0 = Heuristic(start.x, start.y, goal.x, goal.y);
        //openにスタートマスを登録
        open.Push(new Node(start.x, start.y, 0, h0));

        while (open.Count > 0)
        {
            //ヘルパークラスの探索中断tokenがキャンセルされてないかのチェック
            token.ThrowIfCancellationRequested();

            //登録されたopenされたNodeの取得
            Node current = open.Pop();

            // 既に確定済みならスキップ
            //そのマスは既に調べられていたらこのマスを無視して続ける
            if (closed[current.x, current.z])
                continue;

            // ゴール到達
            //調べる対象のNodeがゴール地点だったら探索を終える
            if (current.x == goal.x && current.z == goal.y)
            {
                //今まで登録してきた地点の配列(parent)を渡す、parentをたどっていったときにその値がstartだったら終わるってのと
                //現在の対象のNodeはgoalを意味するのでgoalを引数に渡す
                return BuildPathFromParents(parent, start, goal);
            }

            //対象マスはcloseして対象外に
            closed[current.x, current.z] = true;

            // 近傍4方向
            for (int i = 0; i < dirs.Length; i++)
            {
                int nx = current.x + dirs[i].x;
                int nz = current.z + dirs[i].y;

                //ここから下以降の処理でもし選ばれてないマスをcloseした場合、最短と思われていたルートが結果的に通れないなどで最短じゃないことが
                //判明した場合、そこで処理が終わってしまって経路探索を完遂することができないから
                //マップの範囲内、または壁、または既に調べられてたらその方向のマスはスキップ
                if (!InRange(nx, nz)) continue;
                if (IsWall(nx, nz)) continue;
                if (closed[nx, nz]) continue;
                int newG = gCost[current.x, current.z] + 1; // スタートから次のマスまでの歩数（実コスト）

                //gCostがまだ未到達の場合はInt.MaxValueが入ってるので確定でこの条件はtrue
                //二回目以降はほかのルート検索時にgCostが入ってる場合があるので条件がtrueかどうかはわからない
                //例:前回(gCost[nx, nz])は総コスト７だった、しかし今回(newG)は総コスト４だった
                if (newG < gCost[nx, nz])
                {

                    gCost[nx, nz] = newG;//値を総コストが低い方に更新
                    int h = Heuristic(nx, nz, goal.x, goal.y);//ゴールまでの距離（推定コスト）
                    int f = newG + h;//実コスト＋推定コスト（最終コスト）
                    open.Push(new Node(nx, nz, newG, f));

                    parent[nx, nz] = new Vector2Int(current.x, current.z);
                }
            }
        }

        // 経路なし
        //ここはつまり、PopしていったらopenのNodeは一つ減らされる=>これが続いたがopenのnodeはないしどの経路をたどってもゴールにいく道がなかった時
        return null;
    }

    // 経路復元
    List<Vector2Int> BuildPathFromParents(Vector2Int[,] parent, Vector2Int start, Vector2Int goal)
    {
        var path = new List<Vector2Int>();
        Vector2Int current = goal;

        // 安全装置（バグって無限ループしないように）
        int guard = width * height + 10;

        while (!(current.x == start.x && current.y == start.y) && guard-- > 0)
        {
            //parentがstart地点じゃない間経路を戻り続ける、その値をpathに格納していく
            path.Add(current);
            Vector2Int p = parent[current.x, current.y];
            current = p;
        }

        // スタートも入れる
        path.Add(start);
        //リストの最初がゴール地点になっているのでリストの中身を逆にする
        path.Reverse();
        return path;
    }

    // マンハッタン距離
    //推定コストの計算、現在地から目的地までのマス目ベースの距離を返す、斜め移動はないので横何マス分、縦何マス分かの合計
    int Heuristic(int x, int z, int gx, int gz)
    {
        return Mathf.Abs(x - gx) + Mathf.Abs(z - gz);
    }

    bool InRange(int x, int z)
    {
        return (x >= 0 && z >= 0 && x < width && z < height);
    }

    bool IsWall(int x, int z)
    {
        return map[x, z] == 1;
    }

    // ===== A* 用のノード構造体 =====

    struct Node
    {
        public int x;
        public int z;
        public int g; // 実コスト(スタートからの歩数)
        public int f; // f = g + h

        public Node(int x, int z, int g, int f)
        {
            this.x = x;
            this.z = z;
            this.g = g;
            this.f = f;
        }
    }

    // ===== 最小ヒープ（優先度付きキュー） =====
    //親の f は、必ず 子の f 以下でなければならない、ないし逆、これをヒープ条件 っていうらしい
    //ヒープの処理は普通のいちいち比較していく処理に加えて早いからこれを採用
    class MinHeap
    {
        private Node[] data;
        private int count;

        public int Count => count;

        public MinHeap(int capacity = 64)
        {
            data = new Node[capacity];
            count = 0;
        }

        //openの配列の要素数を増やす
        private void EnsureCapacity()
        {
            if (count >= data.Length)
            {
                Array.Resize(ref data, data.Length * 2);
            }
        }

        //openにマスを登録
        public void Push(Node node)
        {
            EnsureCapacity();

            //カウントを１追加、＋されたものをiに代入しないのは配列は０からスタートだから例えば要素数が３個だとしても0,1,2となるのでi自体は２になる
            //またこのヒープの処理は配列のインデックスにならって行うから++countだとおかしくなる
            //また、EnsureCapacityでResizeしてるので配列がIndexOutOfRangeになる心配もない
            int i = count++;
            //現在のopenの最後尾にnodeを代入
            data[i] = node;

            // 上方向にヒープ調整
            //ここで参照するのはi = 実際のopenされたNodeの数、だからEnsureCapacityで追加された値は読まない
            //だからSortOrderとか使うと容量追加で増えたdefault値が前に来て破綻する＋そもそもこれはソートが目的じゃないから必要ない
            while (i > 0)                //数字　＝　i 
            {
                //childLeft = 2i + 1,childRight = 2(i + 1)       0
                // iはここのiとは無関係                      1       2
                　　　　　　　　　　　　　　　　　　　　//3      4  5    6

                //parent =  childLeft - 1 / 2,parent = childRight - 2 / 2
                //parentについて解いた時は式違うけどint同士の計算の性質上とこのヒープ構造の性質上で親は
                //どちらの子供だとしても同じ親だから int parent = (i - 1) / 2;
                int parent = (i - 1) / 2;
                //親の値が自分の合計コストよりも小さいか同じだったら終了、
                //ヒープソートが目的じゃないからそこだけ見ればいい
                if (data[parent].f <= data[i].f) break;

                //左側に「入れ替えたい変数」,右側に「入れ替え元の値」モダンなC# switchのやつと同じ感じのやつかな
                //一行で入れ替えできる神
                (data[parent], data[i]) = (data[i], data[parent]);
                //親と子を入れ替えたから次の自分はparentの位置、だからi = parent
                i = parent;
            }
        }

        //openの中で最も総コストの低いものが選ばれる
        public Node Pop()
        {
            //Pushの時点でもう先頭が最小値なのは確定してるからとりあえずそれを取り出す。
            Node root = data[0];
            //Popされるものはcloseされるのが確定だからカウントを減らす
            count--;

            if (count > 0)
            {
                //data[0]を末尾のもので上書き、実質的にdata[0]を消去
                //また、末尾はヒープの構造を壊さない唯一のノードだからこれが可能
                data[0] = data[count];

                // 下方向にヒープ調整
                int i = 0;
                while (true)
                {
                    int left = i * 2 + 1;//左の子のインデックス
                    int right = left + 1;//右の子のインデックス
                    if (left >= count) break;//左の子（ヒープでインデックスが小さい方）がopenの数の上限値だったら終わり

                    //仮置き、次の処理で
                    int smallest = left;

                    //右の子がヒープの末端じゃないかつ右の子の総コストが左の子の総コストより小さい
                    if (right < count && data[right].f < data[left].f)
                    {
                        smallest = right;
                    }

                    //現在の末尾の値　data[i].f　がiから見て最も小さい値をもつ子よりもちいさかったら終了
                    if (data[i].f <= data[smallest].f) break;
                    //末尾の値が子よりも大きかったら子供と末尾の値を入れ替える
                    (data[i], data[smallest]) = (data[smallest], data[i]);
                    //現在の末尾の位置をiに保存
                    i = smallest;
                }
            }

            return root;
        }
    }
}
