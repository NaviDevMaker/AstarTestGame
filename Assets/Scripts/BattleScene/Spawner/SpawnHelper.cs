using UnityEngine;
using Game.Stage;
using Game.SpawnableObj;
using Cysharp.Threading.Tasks;

namespace Game.SpawnableObj
{
    public interface ISpawnableObj 
    {
        GameObject ownerObj { get;}
    }
}

namespace Game.Spawner
{
    public interface ISpawner
    {
        void Spawn(int targetX, int targetY);
        SpawnerType spawnerType { get;}

        ISpawnableObj  GetTargetObj();
        void Initialize();
    }
    public enum SpawnerType
    {
        Item,
        Enemy
    }

    public class SpawnHelper<TPrefabData> : IAssetSetter where TPrefabData : SpawnDataBase
    {
        public int[,] occupyMap { get; private set; }
        float spawnTime = 0f;
        float elapsedTime = 0f;
        string address;
        public TPrefabData prefabGenerateSetting { get; private set;}
        SpawnerType spawnerType;
        public SpawnHelper(SpawnerType spawnerType) => Initialize(spawnerType).Forget();
        async UniTask Initialize(SpawnerType spawnerType)
        {
            this.spawnerType = spawnerType;
            var map = StageGenerator.Instance.map;
            var xLength = map.GetLength(0);
            var yLength = map.GetLength(1);
            occupyMap = new int[xLength, yLength];
            address = GetAddress(spawnerType);
            if (address == default(string)) throw new System.NullReferenceException();
            await GetAsset();
            this.spawnTime = prefabGenerateSetting.SpawnTime;
        }
        public bool IsReachedSpawnTime()
        {
            if (prefabGenerateSetting == null) return false;
            elapsedTime += Time.deltaTime;
            return elapsedTime >= spawnTime;
        }
        public bool IsSpawnable(out Vector2Int node)
        {
            node = Vector2Int.zero;
            var randomNode = StageMethods.GetRandomNode();
            var targetX = randomNode.x;
            var targetY = randomNode.y;
            var isWall = StageMethods.IsWall(targetX, targetY);
            var isSpawnable = spawnerType == SpawnerType.Item 
                                             ? IsSpawnableOnMap(targetX, targetY)
                                             : true;
            if (isWall || !isSpawnable) return false;
            elapsedTime = 0f;
            node = new Vector2Int(targetX, targetY);
            return true;
        }
        bool IsSpawnableOnMap(int targetX, int targetY) => occupyMap[targetX, targetY] == 0;

        string GetAddress(SpawnerType spawnerType)
        {
            return spawnerType switch
            { 
                SpawnerType.Item => "Datas/ItemData/ItemGenerateSetting",
                SpawnerType.Enemy => "Datas/SpawnDatas/EnemyGenerateSetting",
                _=> default
            };
        }
        public async UniTask GetAsset() => prefabGenerateSetting  = (TPrefabData)await GetAssetsMethods.GetAsset<SpawnDataBase>(address);


    }

}


