using UnityEngine;

namespace Game.Stage
{
    public struct MapPositionInfo
    {
        public int targetX;
        public int targetY;
        public Vector3 scale;
        public StageGenerator.ObjectState wallSetting => StageGenerator.Instance._WallSetting;
        public MapPositionInfo(int targetX,int targetY)
        {
            var instance = StageGenerator.Instance;
            this.targetX = targetX;
            this.targetY = targetY;
            var map = instance.map;
            var index = map[targetX, targetY];
            this.scale = instance.mapObjects[index].transform.localScale;
        }
    }
    public static class StageMethods
    {
        static int[,] _map => StageGenerator.Instance.map;
        public static bool IsGrounded(int targetX,int targetY)
        {
            var node = _map[targetX, targetY];
            return node == (int)objectType.ground;
        }
        public static bool IsWall(int targetX,int targetY)
        {
            var node = _map[targetX, targetY];
            return node == (int)objectType.wall;
        }
        public static bool IsRoad(int targetX,int targetY)
        {
            var node = _map[targetX, targetY];
            return node == (int)objectType.road;
        }
        public static Vector3 GetTargetNodePos(MapPositionInfo mapPositionInfo)
        {
            var map = StageGenerator.Instance.map;
            var scale = mapPositionInfo.scale;
            var targetX = mapPositionInfo.targetX;
            var targetY = mapPositionInfo.targetY;
            var node = map[targetX,targetY];
            var defaultPosition = StageGenerator.Instance.defaultPosition;

            return node switch
            {
                // defaultPosition.x + nowW * scale.x ここまでの計算で終わるとcubeは座標の位置がそのcubeの中
                //scale.x * 0.5fを足すことによってcubeの左端がmapの位置をワールドに変換したときの位置に来るようになる
                (int)objectType.wall => new Vector3(
                        defaultPosition.x + targetX * scale.x + scale.x * 0.5f,
                        defaultPosition.y + ((mapPositionInfo.wallSetting.size.y - 1) * 0.5f),
                        defaultPosition.z + targetY * scale.z + scale.z * 0.5f),
                (int)objectType.ground => new Vector3(
                        defaultPosition.x + targetX * scale.x + scale.x * 0.5f,
                        defaultPosition.y,
                        defaultPosition.z + targetY * scale.z + scale.z * 0.5f),
                (int)objectType.road => new Vector3(defaultPosition.x + targetX * scale.x + scale.x * 0.5f,
                        defaultPosition.y,
                        defaultPosition.z + targetY * scale.z + scale.z * 0.5f),
                _ => default(Vector3)
            };
        }

        /// <summary>
        /// ランダムなmapの座標をタプルで返す、二次元配列で返さないのは他のメソッドをこの値をもとに使えるようにするため
        /// </summary>
        /// <returns></returns>
        public static (int x,int y) GetRandomNode()
        {
            var mapSizeX = _map.GetLength(0);
            var mapSizeY = _map.GetLength(1);
            var randomX = Random.Range(0,mapSizeX);
            var randomY = Random.Range(0, mapSizeY);
            return (randomX, randomY);
        }
    }
}


