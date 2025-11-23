
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Stage
{
    public class TreeSpawner : MonoBehaviour
    {
        [SerializeField] GameObject treePrefab;

        [SerializeField] int margin_NextToMaxSize;
        [SerializeField] int margin_NextToWall;
        [SerializeField] int treeCount;

        Terrain terrain;
        int spawnTreeCount;
        enum DirectionType
        { 
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
        }

        public void SpawnTreeAroundStage(int mapSizeW,int mapSizeH,Vector3 defaultPos)
        {
            var treeParent = new GameObject("TreeParent").transform;
            spawnTreeCount = treeCount / 4;
            terrain = Terrain.activeTerrain;
            var terrainPos = terrain.transform.position;
            var terrainSize = terrain.terrainData.size;
            for (int i = 0; i < Enum.GetValues(typeof(DirectionType)).Length; i++)
            {
                DirectionType directionType = (DirectionType)i;
                var tople = GetMinAndMax(terrainPos,mapSizeW, mapSizeH, terrainSize,defaultPos,directionType);
                SpawnTree(tople.minX,tople.minZ,tople.maxX,tople.maxZ,treeParent);
            }
        }
        List<GameObject> SpawnTree(int minX,int minZ,int maxX,int maxZ,Transform parent)
        {
            var treeList = new List<GameObject>();
            for (int i = 0; i < spawnTreeCount; i++)
            {
                var x = UnityEngine.Random.Range(minX,maxX);
                var z = UnityEngine.Random.Range(minZ, maxZ);
                var spawnPos = new Vector3(x,0f,z);
                spawnPos.y = Terrain.activeTerrain.SampleHeight(spawnPos);
                var tree = Instantiate(treePrefab,spawnPos,Quaternion.identity,parent);
                treeList.Add(tree);
            }

            return treeList;
        }
        (int minX,int minZ,int maxX,int maxZ) GetMinAndMax(Vector3 terrainPos,int mapSizeW,
                                              int mapSizeH,Vector3 terrainSize, Vector3 defaultPos, DirectionType directionType)
        {
            var minX = default(int);
            var minZ = default(int);
            var maxX = default(int);
            var maxZ = default(int);
            Func<(int minX, int minZ, int maxX,int maxZ)> minAndMax = directionType switch
            {
                DirectionType.Right => () =>
                {
                    minX = Mathf.RoundToInt(defaultPos.x + mapSizeW + margin_NextToWall);
                    minZ = Mathf.RoundToInt(terrainPos.z + margin_NextToMaxSize);
                    maxX = Mathf.RoundToInt(terrainPos.x + terrainSize.x - margin_NextToMaxSize);
                    maxZ = Mathf.RoundToInt(terrainPos.z + terrainSize.z - margin_NextToMaxSize);
                    return (minX, minZ, maxX, maxZ);
                }
                ,
                DirectionType.Left => () =>
                {
                    minX = Mathf.RoundToInt(terrainPos.x + margin_NextToMaxSize);
                    minZ = Mathf.RoundToInt(terrainPos.z + margin_NextToMaxSize);
                    maxX = Mathf.RoundToInt(defaultPos.x - margin_NextToWall);
                    maxZ = Mathf.RoundToInt(terrainPos.z + terrainSize.z - margin_NextToMaxSize);
                    return (minX, minZ, maxX, maxZ);
                },
                DirectionType.Bottom => () =>
                {
                    minX = Mathf.RoundToInt(defaultPos.x);
                    minZ = Mathf.RoundToInt(terrainPos.z + margin_NextToMaxSize);
                    maxX = Mathf.RoundToInt(defaultPos.x + mapSizeW);
                    maxZ = Mathf.RoundToInt(terrainPos.z + defaultPos.z + margin_NextToWall);
                    return (minX, minZ, maxX, maxZ);
                }
                ,
                DirectionType.Top => () =>
                {
                    minX = Mathf.RoundToInt(defaultPos.x);
                    minZ = Mathf.RoundToInt(defaultPos.z + mapSizeH + margin_NextToWall);
                    maxX = Mathf.RoundToInt(defaultPos.x + mapSizeW);
                    maxZ = Mathf.RoundToInt(terrainPos.z + terrainSize.z - margin_NextToMaxSize);
                    return (minX, minZ, maxX, maxZ);
                }
                ,
                _=> default
            };

            return minAndMax();
        }
    }
}


