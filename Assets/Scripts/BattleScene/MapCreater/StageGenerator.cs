using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StageGenerator : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] ObjectState WallSetting;
    [SerializeField] ObjectState GroundSetting;
    [SerializeField] ObjectState RoadSetting;

    Vector3 defaultPosition;

    [Serializable]
    class ObjectState
    {
        public Color color;
        public Vector3 size;
    }

    private int mapSizeW;        // マップの横サイズ
    private int mapSizeH;        // マップの縦サイズ
    private int[,] map;                           // マップの管理配列

    [SerializeField] private int roomNum;       // 部屋の数
    private int roomMin = 10;                   // 生成する部屋の最小値,これは部屋の幅、高さの最小値
    private int parentNum = 0;                  // 分割する部屋番号
    private int max = 0;                        // 最大面積
    private int roomCount;                      // 部屋カウント
    private int line = 0;                       // 分割点
    private int[,] roomStatus;                  // 部屋の管理配列

    private enum RoomStatus                     // 部屋の配列ステータス
    {
        x,// 部屋の左下のX座標
        y,// 部屋の左下のY座標
        w,// 部屋の横幅（Width）
        h,/// 部屋の縦幅（Height）

        rx,// 実際に部屋を生成するX
        ry,// 実際に部屋を生成するY
        rw,// 実際の部屋の幅
        rh, // 実際の部屋の高さ
    }

    enum objectType
    {
        ground = 0,
        wall = 1,

        road = 2,
    }

    private GameObject[] mapObjects;               // マップ生成用のオブジェクト配列
    private GameObject[] objectParents;             // 各タイプ別の親オブジェクト配列

    private const int offsetWall = 2;   // 壁から離す距離
    private const int offset = 1;       // 調整用

    public void Initialize()
    {
        SetMapsizeAndPosition();
        MapGenerate();
    }
    void SetMapsizeAndPosition()
    {
        TerrainData terrainData = terrain.terrainData;
        mapSizeW = Mathf.RoundToInt(terrainData.size.x);
        mapSizeH = Mathf.RoundToInt(terrainData.size.z);
        Debug.Log($"Terrain Size = {mapSizeW} x {terrainData.size.y} x {mapSizeH}");
        defaultPosition = terrain.GetPosition();
    }
    // 生成用のオブジェクトを用意
    void initPrefab()
    {
        // 親オブジェクトの生成
        GameObject groundParent = new GameObject("Ground");
        GameObject wallParent = new GameObject("Wall");
        GameObject roadParent = new GameObject("Road");

        // 配列にプレハブを入れる
        objectParents = new GameObject[] { groundParent, wallParent, roadParent };

        // 迷路オブジェクトの初期化
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.transform.localScale = GroundSetting.size;
        ground.GetComponent<Renderer>().material.color = GroundSetting.color;
        ground.name = "ground";
        ground.transform.SetParent(objectParents[(int)objectType.ground].transform);
        ground.SetActive(false);

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.localScale = WallSetting.size;
        wall.GetComponent<Renderer>().material.color = WallSetting.color;
        wall.name = "wall";
        wall.transform.SetParent(objectParents[(int)objectType.wall].transform);
        wall.SetActive(false);

        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.transform.localScale = RoadSetting.size;
        road.GetComponent<Renderer>().material.color = RoadSetting.color;
        road.name = "road";
        road.transform.SetParent(objectParents[(int)objectType.road].transform);
        wall.SetActive(false);
        // 配列にプレハブを入れる
        mapObjects = new GameObject[] { ground, wall, road };
    }

    //イメージは大きな図形を分解してるイメージ、だからまだ実際の位置は未確定
    void SplitedRoomSetup()
    {
        // 部屋（StartX、StartY、幅、高さ）
        roomStatus = new int[System.Enum.GetNames(typeof(RoomStatus)).Length, roomNum];

        

        // フロアを入れる
        //roomCountは最初は０(まだ何も生成されてないから)で初期化、ここで部屋一つ目の生成
        roomStatus[(int)RoomStatus.x, roomCount] = 0;　//x座標
        roomStatus[(int)RoomStatus.y, roomCount] = 0;　//y座標
        roomStatus[(int)RoomStatus.w, roomCount] = mapSizeW;　//部屋の横幅
        roomStatus[(int)RoomStatus.h, roomCount] = mapSizeH;　//部屋の縦幅

        // カウント追加
        //初期化（一つ目の部屋の生成）したのでroomCountをインクリメント
        roomCount++;

        //例（一番最初の場合)
        //width,heightがともに１００（terrainのサイズのwidth,lengthがともに100の時）に、当然ほかのインデックスの各値（RoomStateの各要素）には
        //何も入ってないので、maxに100×100の面積10000が入り、parentNumにも１が代入される
        //SplitPointでその親となる部屋のwidthとheightを渡す、そして値の大きな方(辺の長い方)をRandom.rangeの値をもとに分割する座標(line)を取得する
        //生成予定の部屋(roomCount)が分割した際の左側になるように値を入れる①
        // 部屋の数だけ分割する
        //splitNumは分割した回数
        for (int splitNum = 0; splitNum < roomNum - 1; splitNum++)
        {
            // 変数初期化
            parentNum = 0;  // 分割する部屋番号
            max = 0;        // 最大面積

            // 最大の部屋番号を調べる
            for (int maxCheck = 0; maxCheck < roomNum; maxCheck++)
            {
                // 面積比較
                //roomStatus[(int)RoomStatus.w, maxCheck] * roomStatus[(int)RoomStatus.h, maxCheck] が面積、maxCheckが部屋番号

                if (max < roomStatus[(int)RoomStatus.w, maxCheck] * roomStatus[(int)RoomStatus.h, maxCheck])
                {
                    // 最大面積上書き
                    max = roomStatus[(int)RoomStatus.w, maxCheck] * roomStatus[(int)RoomStatus.h, maxCheck];

                    // 親の部屋番号セット,現在一番でかい部屋のインデックスを登録
                    parentNum = maxCheck;
                }
            }
            //①
            // 取得した部屋をさらに割る
            if (SplitPoint(roomStatus[(int)RoomStatus.w, parentNum], roomStatus[(int)RoomStatus.h, parentNum]))
            {
                //横幅がでかい場合
                // 取得
                roomStatus[(int)RoomStatus.x, roomCount] = roomStatus[(int)RoomStatus.x, parentNum];
                roomStatus[(int)RoomStatus.y, roomCount] = roomStatus[(int)RoomStatus.y, parentNum];
                //line(割る位置)の分を引いてその値（width）を規定のルームカウントのwidthにいれる
                roomStatus[(int)RoomStatus.w, roomCount] = roomStatus[(int)RoomStatus.w, parentNum] - line;
                roomStatus[(int)RoomStatus.h, roomCount] = roomStatus[(int)RoomStatus.h, parentNum];

                // 親の部屋を整形する
                //親となっていた部屋が分割した際の右側になるように調整する
                roomStatus[(int)RoomStatus.x, parentNum] += roomStatus[(int)RoomStatus.w, roomCount];//生成予定のルームのwidth分座標を右に移動
                roomStatus[(int)RoomStatus.w, parentNum] -= roomStatus[(int)RoomStatus.w, roomCount];//widthを生成予定のルームのwidth分減らす
            }
            else
            {
                //横幅の分割の際と同じ
                //縦幅がでかい場合
                // 取得
                roomStatus[(int)RoomStatus.x, roomCount] = roomStatus[(int)RoomStatus.x, parentNum];
                roomStatus[(int)RoomStatus.y, roomCount] = roomStatus[(int)RoomStatus.y, parentNum];
                roomStatus[(int)RoomStatus.w, roomCount] = roomStatus[(int)RoomStatus.w, parentNum];
                //line(割る位置)の分を引いてその値（height）を規定のルームカウントのheightにいれる
                roomStatus[(int)RoomStatus.h, roomCount] = roomStatus[(int)RoomStatus.h, parentNum] - line;

                // 親の部屋を整形する
                roomStatus[(int)RoomStatus.y, parentNum] += roomStatus[(int)RoomStatus.h, roomCount];
                roomStatus[(int)RoomStatus.h, parentNum] -= roomStatus[(int)RoomStatus.h, roomCount];
            }
            // カウントを加算
            roomCount++;
        }

    }

    void MapInit()
    {
        // フロア設定
        map = new int[mapSizeW, mapSizeH];

        // フロアの初期化
        for (int nowW = 0; nowW < mapSizeW; nowW++)
        {
            for (int nowH = 0; nowH < mapSizeH; nowH++)
            {
                // 壁を貼る
                map[nowW, nowH] = 2;//これはobjectTypeのroadとは関係ない
                //イメージ図 　各マップのノードに置くものに2を置いてるイメージ　後に0や1といったobjectTypeのものに変えていく
                /*222222222222
                  222222222222
                  222222222222
                  222222222222
                  222222222222
                  222222222222
                */
            }
        }
    }

    void DecideRoomScallAndPosition()
    {
        // 分割した中にランダムな大きさの部屋を生成

        for (int i = 0; i < roomNum; i++)
        {
            //生成座標の設定
            int x = roomStatus[(int)RoomStatus.x, i];
            int y = roomStatus[(int)RoomStatus.y, i];
            int w = roomStatus[(int)RoomStatus.w, i];
            int h = roomStatus[(int)RoomStatus.h, i];

            //Q :なんでoffsetWallを引くのか、offsetを大きさで引くから必要ないんじゃないのか？
            //A :大きさ決定時のRandom.Rangeの時に最大値のほうが小さくなってエラーを吐く可能性が大きくなってしまうから

            //最小値はoffsetWall(壁の分の余白)＋ ルームステータス(仮置きのｘ座標),最大値はroomの横幅、縦幅の最小値（roomMin）＋ offsetWall
            roomStatus[(int)RoomStatus.rx, i] = Random.Range(x + offsetWall, (x + w) - (roomMin + offsetWall));
            roomStatus[(int)RoomStatus.ry, i] = Random.Range(y + offsetWall, (y + h) - (roomMin + offsetWall));

            //最小値はroomMin(高さ、幅の最小値),最大値はroomの横幅 - （さっき決めた実際の座標 - roomのX座標）=> これによりそのroom内でのローカルな座標が求まる
            //offsetWallではなくoffsetを引いているのは最小余白のため
            roomStatus[(int)RoomStatus.rw, i] = Random.Range(roomMin, w - (roomStatus[(int)RoomStatus.rx, i] - x) - offset);
            roomStatus[(int)RoomStatus.rh, i] = Random.Range(roomMin, h - (roomStatus[(int)RoomStatus.ry, i] - y) - offset);
        }

    }

    void OverrideMap()
    {
        // マップ上書き
        for (int count = 0; count < roomNum; count++)
        {
            // 取得した部屋の確認
            for (int h = 0; h < roomStatus[(int)RoomStatus.h, count]; h++)
            {
                for (int w = 0; w < roomStatus[(int)RoomStatus.w, count]; w++)
                {
                    // 部屋チェックポイント
                    //[w + roomStatus[(int)RoomStatus.x, count] = 部屋生成前のx、h + roomStatus[(int)RoomStatus.y, count] = 部屋生成前のh
                    //の各ノードをforループして一旦wall(1)にする
                    map[w + roomStatus[(int)RoomStatus.x, count], h + roomStatus[(int)RoomStatus.y, count]] = 1;
                }

            }

            // 生成した部屋
            for (int h = 0; h < roomStatus[(int)RoomStatus.rh, count]; h++)
            {
                for (int w = 0; w < roomStatus[(int)RoomStatus.rw, count]; w++)
                {
                    //[w + roomStatus[(int)RoomStatus.rx, count] = 生成された部屋の実際のx、h + roomStatus[(int)RoomStatus.ry, count] = 生成された部屋の実際のh
                    //の各ノードをforループで床にする
                    map[w + roomStatus[(int)RoomStatus.rx, count], h + roomStatus[(int)RoomStatus.ry, count]] = 0;
                }
            }
        }

    }
    void ShortRoadCreate()
    {
        // 道の生成
        int[] splitLength = new int[4];
        int roodPoint = 0;// 道を引く場所

        // 部屋から一番近い境界線を調べる(十字に調べる)
        for (int nowRoom = 0; nowRoom < roomNum; nowRoom++)
        {
            // 左の壁からの距離 => 生成された部屋の左側から生成前の部屋の端までの距離
            splitLength[0] = roomStatus[(int)RoomStatus.x, nowRoom] > 0 ? // => マップの左端じゃないかどうかの条件確認
                roomStatus[(int)RoomStatus.rx, nowRoom] - roomStatus[(int)RoomStatus.x, nowRoom] : int.MaxValue;
            // 右の壁からの距離 =>  生成された部屋の右側から生成前の部屋の端までの距離
            splitLength[1] = (roomStatus[(int)RoomStatus.x, nowRoom] + roomStatus[(int)RoomStatus.w, nowRoom]) < mapSizeW ? //=> マップの右端じゃないかどうかの条件確認
                (roomStatus[(int)RoomStatus.x, nowRoom] + roomStatus[(int)RoomStatus.w, nowRoom]) - (roomStatus[(int)RoomStatus.rx, nowRoom] + roomStatus[(int)RoomStatus.rw, nowRoom]) : int.MaxValue;

            // 下の壁からの距離 => 生成された部屋の下側から生成前の部屋の端までの距離
            splitLength[2] = roomStatus[(int)RoomStatus.y, nowRoom] > 0 ?　//=> マップの下端じゃないかどうかの条件確認
                roomStatus[(int)RoomStatus.ry, nowRoom] - roomStatus[(int)RoomStatus.y, nowRoom] : int.MaxValue;
            // 上の壁からの距離　=> 生成された部屋の上側から生成前の部屋の端までの距離
            splitLength[3] = (roomStatus[(int)RoomStatus.y, nowRoom] + roomStatus[(int)RoomStatus.h, nowRoom]) < mapSizeH ?//=> マップの上端じゃないかどうかの条件確認
                (roomStatus[(int)RoomStatus.y, nowRoom] + roomStatus[(int)RoomStatus.h, nowRoom]) - (roomStatus[(int)RoomStatus.ry, nowRoom] + roomStatus[(int)RoomStatus.rh, nowRoom]) : int.MaxValue;

            // マックスでない物のみ先へ
            for (int j = 0; j < splitLength.Length; j++)
            {
                if (splitLength[j] != int.MaxValue)
                {
                    // 左右判定, {0,1} 左と右の距離に基づく処理
                    if (j < 2)
                    {
                        // 道を引く場所を決定,ランダムな高さを設定（生成された部屋の中でのランダムな高さ）
                        roodPoint = Random.Range(roomStatus[(int)RoomStatus.ry, nowRoom] + offset, roomStatus[(int)RoomStatus.ry, nowRoom] + roomStatus[(int)RoomStatus.rh, nowRoom] - offset);

                        // マップに書き込む
                        //境界線の距離の分,mapの各ノード（road = 2）に上書きしていく
                        //この処理は壁を道に変えていく処理でこのアルゴリズムは床を上書きしてはいけない
                        for (int w = 1; w <= splitLength[j]; w++)
                        {
                            // 左右判定
                            if (j == 0)　//左の場合
                            {
                                // 左
                                //左の場合はrx(実際の部屋の座標x)からw分引いていった値がmapのx座標になる
                                //roomStatus[(int)RoomStatus.rx, nowRoom]は左端（図形の始点）
                                //左側はrxが始点であり既に床であり、配列的な考えはないのでoffset等の計算は不要
                                map[(-w) + roomStatus[(int)RoomStatus.rx, nowRoom], roodPoint] = 2;
                            }
                            else //右の場合
                            {
                                // 右
                                //roomStatus[(int)RoomStatus.rx, nowRoom] + roomStatus[(int)RoomStatus.rw, nowRoom] = 壁の位置　-offsetで床の最右、wを足して壁の最初の部分になる
                                //roomStatus[(int)RoomStatus.rw, nowRoom]は要素数（マスの数）だから配列的に考えてrx = 0の時、widthが５のとき、0,1,2,3,4,5となりこれだと6個になる。これは本来のwidth,5として扱えていない
                                //だからoffset分を引いている
                                map[w + roomStatus[(int)RoomStatus.rx, nowRoom] + roomStatus[(int)RoomStatus.rw, nowRoom] - offset, roodPoint] = 2;

                                // 最後
                                if (w == splitLength[j])
                                {
                                    // 一つ多く作る
                                    map[w + offset + roomStatus[(int)RoomStatus.rx, nowRoom] + roomStatus[(int)RoomStatus.rw, nowRoom] - offset, roodPoint] = 2;
                                }
                            }
                        }
                    }
                    else　// 上下判定,{2,3}  上と下の距離に基づく処理
                    {
                        // 道を引く場所を決定
                        //左右同様だが、今回はxの始点を決める
                        roodPoint = Random.Range(roomStatus[(int)RoomStatus.rx, nowRoom] + offset, roomStatus[(int)RoomStatus.rx, nowRoom] + roomStatus[(int)RoomStatus.rw, nowRoom] - offset);

                        // マップに書き込む
                        for (int h = 1; h <= splitLength[j]; h++)
                        {
                            // 上下判定
                            if (j == 2)
                            {
                                //下
                                //同様始点が床にする
                                map[roodPoint, (-h) + roomStatus[(int)RoomStatus.ry, nowRoom]] = 2;
                            }
                            else
                            {
                                //上
                                //左右同様offset分を引いて始点を床に指定
                                map[roodPoint, h + roomStatus[(int)RoomStatus.ry, nowRoom] + roomStatus[(int)RoomStatus.rh, nowRoom] - offset] = 2;

                                // 最後
                                if (h == splitLength[j])
                                {
                                    // 一つ多く作る
                                    map[roodPoint, h + offset + roomStatus[(int)RoomStatus.ry, nowRoom] + roomStatus[(int)RoomStatus.rh, nowRoom] - offset] = 2;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void ConnectEachRoad()
    {
        int roadVec1 = 0;// 道の始点
        int roadVec2 = 0;// 道の終点

        // 道の接続
        // 各部屋の内２辺だけを確認すればいい理由 : BSP（Binary Space Partitioning：二分割）は分割前の親と壁を共有してるから、親の道とつながり結果的にすべての部屋がつながるから
        for (int nowRoom = 0; nowRoom < roomNum; nowRoom++)
        {
            roadVec1 = 0;
            roadVec2 = 0;
            // 道を繋げる
            for (int roodScan = 0; roodScan < roomStatus[(int)RoomStatus.w, nowRoom]; roodScan++)
            {
                // 道を検索
                //境界にある２(road)の部分を探す、この時nowroomだから一見発見されるroadはnowroom(自分自身)が作ったものだと思われるが違う（俺が勘違いしてた）隣接するほかの部屋が作ったroadもある。だからroaaVec1じゃ足りない
                //roadVec2（他者が作ったroad）も探してroadVec1とroadVec2の間のノードを道に変える
                if (map[roodScan + roomStatus[(int)RoomStatus.x, nowRoom], roomStatus[(int)RoomStatus.y, nowRoom]] == 2)
                {
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roodScan + roomStatus[(int)RoomStatus.x, nowRoom];
                    }
                    else
                    {
                        // 終点セット
                        roadVec2 = roodScan + roomStatus[(int)RoomStatus.x, nowRoom];
                    }
                }
            }
            // 道を引く
            for (int roadSet = roadVec1; roadSet < roadVec2; roadSet++)
            {
                // 境界線を上書き
                map[roadSet, roomStatus[(int)RoomStatus.y, nowRoom]] = 2;
            }

            roadVec1 = 0;
            roadVec2 = 0;

            for (int roadScan = 0; roadScan < roomStatus[(int)RoomStatus.h, nowRoom]; roadScan++)
            {
                // 道を検索
                if (map[roomStatus[(int)RoomStatus.x, nowRoom], roadScan + roomStatus[(int)RoomStatus.y, nowRoom]] == 2)
                {
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roadScan + roomStatus[(int)RoomStatus.y, nowRoom];
                    }
                    else
                    {
                        // 終点セット
                        roadVec2 = roadScan + roomStatus[(int)RoomStatus.y, nowRoom];
                    }
                }
            }
            // 道を引く
            for (int roadSet = roadVec1; roadSet < roadVec2; roadSet++)
            {
                // 境界線を上書き
                map[roomStatus[(int)RoomStatus.x, nowRoom], roadSet] = 2;
            }
        }
    }
    private void MapGenerate()
    {
        initPrefab();

        ///////
        MapInit();
        SplitedRoomSetup();
        DecideRoomScallAndPosition();
        OverrideMap();
        ShortRoadCreate();
        ConnectEachRoad();
        GenerateEachObjects();
    }

    void GenerateEachObjects()
    {
        // オブジェクトを生成する
        for (int nowH = 0; nowH < mapSizeH; nowH++)
        {
            for (int nowW = 0; nowW < mapSizeW; nowW++)
            {

                GameObject mazeObject = InstanciateObject(nowW, nowH);
                mazeObject.SetActive(true);
                // 壁の生成
                //if (map[nowW, nowH] == (int)objectType.wall)
                //{
                //    GameObject mazeObject = Instantiate(
                //        cube,
                //        new Vector3(
                //            defaultPosition.x + nowW * scale.x + scale.x * 0.5f,
                //            defaultPosition.y + ((WallSetting.size.y - 1) * 0.5f),
                //            defaultPosition.z + nowH *scale.z + scale.z * 0.5f),
                //        Quaternion.identity, parent);
                //    mazeObject.SetActive(true);
                //}

                //// 部屋の生成
                //if (map[nowW, nowH] == (int)objectType.ground)
                //{
                //    GameObject mazeObject = Instantiate(
                //        cube,
                //        new Vector3(
                //            defaultPosition.x + nowW * scale.x + scale.x * 0.5f,
                //            defaultPosition.y,
                //            defaultPosition.z + nowH * scale.z + scale.z * 0.5f),
                //        Quaternion.identity, parent);
                //    mazeObject.SetActive(true);
                //}

                //// 通路の生成
                //if (map[nowW, nowH] == (int)objectType.road)
                //{
                //    GameObject mazeObject = Instantiate(
                //        cube,
                //        new Vector3(
                //            defaultPosition.x + nowW * scale.x + scale.x * 0.5f,
                //            defaultPosition.y,
                //            defaultPosition.z + nowH * scale.z + scale.z * 0.5f),
                //        Quaternion.identity, parent);
                //    mazeObject.SetActive(true);
                //}
            }
        }
    }
    GameObject InstanciateObject(int nowW,int nowH)
    {
        int index = map[nowW, nowH];
        GameObject cube = mapObjects[index];
        Vector3 scale = cube.transform.localScale;
        Transform parent = objectParents[index].transform;
        Quaternion rot = Quaternion.identity;
        Vector3 pos = index switch
        {
            (int)objectType.wall => new Vector3(
                    defaultPosition.x + nowW * scale.x + scale.x * 0.5f,
                    defaultPosition.y + ((WallSetting.size.y - 1) * 0.5f),
                    defaultPosition.z + nowH * scale.z + scale.z * 0.5f),
            (int)objectType.ground => new Vector3(
                    defaultPosition.x + nowW * scale.x + scale.x * 0.5f,
                    defaultPosition.y,
                    defaultPosition.z + nowH * scale.z + scale.z * 0.5f),
            (int)objectType.road => new Vector3(defaultPosition.x + nowW * scale.x + scale.x * 0.5f,
                    defaultPosition.y,
                    defaultPosition.z + nowH * scale.z + scale.z * 0.5f),
            _ => default(Vector3)
        };
        return Instantiate(cube, pos, rot, parent);
    }
    // 分割点のセット(int x, int y)、大きい方を分割する
    private bool SplitPoint(int x, int y)
    {
        //横幅と縦幅のでかい方の比較
        // 分割位置の決定
        if (x > y)
        {
            //横幅がでかい場合
            line = Random.Range(roomMin + (offsetWall * 2), x - (offsetWall * 2 + roomMin));// 縦割り
            return true;
        }
        else
        {
            //縦幅がでかい場合
            line = Random.Range(roomMin + (offsetWall * 2), y - (offsetWall * 2 + roomMin));// 横割り
            return false;
        }
    }

}



