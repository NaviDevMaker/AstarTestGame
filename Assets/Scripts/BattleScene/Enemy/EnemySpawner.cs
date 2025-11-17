using UnityEngine;
using Game.SpawnableObj;
using Game.Stage;
namespace Game.Spawner
{
    public class EnemySpawner : MonoBehaviour,ISpawner
    {
        SpawnHelper<EnemyGenerateSetting> spawnHelper;
        public SpawnerType spawnerType => SpawnerType.Enemy;

        public ISpawnableObj GetTargetObj()
        {
            var prefabSetting = spawnHelper.prefabGenerateSetting;
            var itemLength = prefabSetting.EnemyPrefabs.Count;
            var r = UnityEngine.Random.Range(0, itemLength);
            return prefabSetting.EnemyPrefabs[r];
        }

        public void Initialize() => spawnHelper = new SpawnHelper<EnemyGenerateSetting>(spawnerType);

        public void Spawn(int targetX, int targetY)
        {
            var mapPositionInfo = new MapPositionInfo(targetX, targetY);
            var pos = StageMethods.GetTargetNodePos(mapPositionInfo);
            var spawnableObj = GetTargetObj();
            var prafabObj = spawnableObj.ownerObj;
            var enemy = Instantiate(prafabObj, pos, prafabObj.transform.rotation);
            spawnHelper.occupyMap[targetX, targetY] = 1;
            //var node = new Vector2Int(targetX, targetY);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (spawnHelper.IsReachedSpawnTime())
            {
                if (spawnHelper.IsSpawnable(out var node)) Spawn(node.x, node.y);
            }
        }
    }
}


