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
public class AStarPathFinder
{
    // ===== フィールド =====

    private readonly int[,] map;
    private readonly int width;
    private readonly int height;
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
        Vector2Int startMap = WorldToMap(worldStart);
        Vector2Int goalMap = WorldToMap(worldGoal);

        // ② 重い A* 計算は別スレッドで実行（Unity API 禁止）
        List<Vector2Int> mapPath = await UniTask.RunOnThreadPool(() =>
        {
            return SearchOnGrid(startMap, goalMap, token);
        }, cancellationToken: token);

        if (mapPath == null || mapPath.Count == 0)
        {
            return null;
        }

        // ③ メインスレッドに戻ってから MapToWorld（Terrain 触ってOK）
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
        if (!InRange(start.x, start.y) || !InRange(goal.x, goal.y))
            return null;
        if (IsWall(start.x, start.y) || IsWall(goal.x, goal.y))
            return null;

        // gCost: スタートからの実コスト
        var gCost = new int[width, height];
        // close: 探索完了フラグ
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

        var open = new MinHeap(64);

        // スタートノード登録
        gCost[start.x, start.y] = 0;
        int h0 = Heuristic(start.x, start.y, goal.x, goal.y);
        open.Push(new Node(start.x, start.y, 0, h0));

        while (open.Count > 0)
        {
            token.ThrowIfCancellationRequested();

            Node current = open.Pop();

            // 既に確定済みならスキップ
            if (closed[current.x, current.z])
                continue;

            // ゴール到達
            if (current.x == goal.x && current.z == goal.y)
            {
                return BuildPathFromParents(parent, start, goal);
            }

            closed[current.x, current.z] = true;

            // 近傍4方向
            for (int i = 0; i < dirs.Length; i++)
            {
                int nx = current.x + dirs[i].x;
                int nz = current.z + dirs[i].y;

                if (!InRange(nx, nz)) continue;
                if (IsWall(nx, nz)) continue;
                if (closed[nx, nz]) continue;

                int newG = gCost[current.x, current.z] + 1; // コスト 1 マス固定

                if (newG < gCost[nx, nz])
                {
                    gCost[nx, nz] = newG;
                    int h = Heuristic(nx, nz, goal.x, goal.y);
                    int f = newG + h;
                    open.Push(new Node(nx, nz, newG, f));

                    parent[nx, nz] = new Vector2Int(current.x, current.z);
                }
            }
        }

        // 経路なし
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
            path.Add(current);
            Vector2Int p = parent[current.x, current.y];
            current = p;
        }

        // スタートも入れる
        path.Add(start);
        path.Reverse();
        return path;
    }

    // マンハッタン距離
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
        public int g; // 実コスト
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

        private void EnsureCapacity()
        {
            if (count >= data.Length)
            {
                Array.Resize(ref data, data.Length * 2);
            }
        }

        public void Push(Node node)
        {
            EnsureCapacity();

            int i = count++;
            data[i] = node;

            // 上方向にヒープ調整
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (data[parent].f <= data[i].f) break;

                (data[parent], data[i]) = (data[i], data[parent]);
                i = parent;
            }
        }

        public Node Pop()
        {
            Node root = data[0];
            count--;

            if (count > 0)
            {
                data[0] = data[count];

                // 下方向にヒープ調整
                int i = 0;
                while (true)
                {
                    int left = i * 2 + 1;
                    int right = left + 1;
                    if (left >= count) break;

                    int smallest = left;
                    if (right < count && data[right].f < data[left].f)
                    {
                        smallest = right;
                    }

                    if (data[i].f <= data[smallest].f) break;

                    (data[i], data[smallest]) = (data[smallest], data[i]);
                    i = smallest;
                }
            }

            return root;
        }
    }
}
