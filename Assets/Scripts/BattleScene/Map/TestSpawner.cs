using UnityEngine;
using System.Linq;
using UnityEditor;

public class TestSpawner : MonoBehaviour
{
    private string[] DetailPaths = {

        "Assets/GrassFlowers/Textures/GrassFlowers/grass02.tga",
        };
    void Start()
    {
        // テレインコンポーネント
        Terrain terrain = GetComponent<Terrain>();

        // テレインサイズを設定する
        terrain.terrainData.size = new Vector3(200, 10, 150);

        // 1. ディティールプロトタイプの生成
        DetailPrototype[] detailPrototypes = DetailPaths.Select(path =>
        {
            DetailPrototype detailPrototype = new DetailPrototype();
            detailPrototype.prototypeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            return detailPrototype;
        }).ToArray();

        // 2. ディティールプロトタイプの設定
        terrain.terrainData.detailPrototypes = detailPrototypes;

        // 1. ディティール解像度の設定
        int detailResolution = 1024;
        int detailResolutionPatch = 16;
        terrain.terrainData.SetDetailResolution(detailResolution, detailResolutionPatch);

        // 2. ディティールレイヤーの取得
        int[,] map = terrain.terrainData.GetDetailLayer(0, 0, detailResolution, detailResolution, 0);

        // 3. ディティールレイヤーに密度設定
        for (int y = 0; y < 100; y++)
        {
            for (int x = 100; x < 150; x++)
            {
                if (x % 10 == 0 && y % 10 == 0)
                {
                    map[y, x] = 3;
                }
            }
        }

        // 4. ディティールレイヤーの設定
        terrain.terrainData.SetDetailLayer(0, 0, 0, map);

        // 4. ディティールレイヤーの設定
    }
}



