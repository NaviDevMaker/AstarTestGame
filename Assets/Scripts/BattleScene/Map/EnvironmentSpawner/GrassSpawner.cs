using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Stage;
using System.Xml.Linq;
using UnityEngine;

namespace Game.Stage
{
    public class GrassSpawner : EnvironmentSpawnerBase
    {
        [SerializeField,Range(1,16)] int dentity;
        public override void SpawnObjectAroundStage(EnvironmentSpawnerInfo environmentSpawnerInfo)
        {
           
               var terrain = environmentSpawnerInfo.terrain;
               var data = terrain.terrainData;
               var size = data.size;
               var mapSizeW = environmentSpawnerInfo.mapSizeW;
               var mapSizeH = environmentSpawnerInfo.mapSizeH;
               var defaultPos = environmentSpawnerInfo.defaultPos;
               var detailResolution = data.detailResolution;
               var xBase = 0;
               var yBase = 0;
               int[,] detailLayer = data.GetDetailLayer(xBase, yBase, detailResolution, detailResolution, 0);
                
               for (int x = 0; x < size.x; x++)
               {
                  for (int y = 0; y < size.y; y++)
                  {
                      if (!isSpawnable(x, y, mapSizeW, mapSizeH, defaultPos)) continue;
                      SetDetailLayer(size,x, y,detailResolution,detailLayer);
                  }
                
               }

            Debug.Log($"DetailLaterのlength,{detailLayer.GetLength(0)}");
               data.SetDetailLayer(xBase, yBase, 0, detailLayer);
            Debug.Log("草の本数：" + CountGrass(detailLayer, detailResolution));

            
        }
        int CountGrass(int[,] dl, int res)
        {
            int count = 0;
            for (int y = 0; y < res; y++)
                for (int x = 0; x < res; x++)
                    if (dl[y, x] > 0)
                        count++;
            return count;
        }
        void SetDetailLayer(Vector3 size,int worldX,int worldY
                           ,int detailResolution, int[,] detailLayer)
        {
            //本当によくない、忘れてた感覚、掛け算の本質はその掛ける数側の基準に掛けられる数を合わせるためね
            //だから草マップ基準に替えたいからdetailResolutionをかける
            //ちなみに草マップは正方形ね
            //切り上げね
            var detailX = Mathf.FloorToInt(((float)worldX / size.x) * detailResolution);
            var detailY = Mathf.FloorToInt(((float)worldY / size.y) * detailResolution);

            //Debug.Log($"detailX:{detailX},detailY:{detailY}");
            //左上から何マス目の位置から抽出するか、草マップは左上基準
            //detailMap は[y, x] でアクセスするらしい
            if (detailX < 0 || detailX >= detailResolution ||
                detailY < 0 || detailY >= detailResolution)
            {
                Debug.LogWarning($"範囲外: detailX={detailX}, detailY={detailY}");
                return;
            }
            detailLayer[detailY,detailX] = dentity;
        }
        bool isSpawnable(int x,int y,int mapSizeW,int mapSizeH,Vector3 defaultPos)
        {
            var dflX = defaultPos.x;
            var dflY = defaultPos.z;
            var mapSizeEndY = dflY + mapSizeH;
            var mapSizeEndW = dflX + mapSizeW;
            if (y < dflY || y > mapSizeEndY) return true;
            //yはマップの大きさ内の中での判定になる、だから見るのはxだけ
            if (x < dflX || x > mapSizeEndW) return true;
            return false;
        }
    }
}

