using UnityEngine;
using Game.SpawnableObj;
using Game.Stage;
using System.Collections.Generic;
using Game.Enemy;
using NUnit.Framework.Internal;
using UnityEngine.Events;
namespace Game.Spawner
{
    public class EnemySpawner : MonoBehaviour,ISpawner,ISetUper
    {
        [SerializeField] int spawnableCount;
        SpawnHelper<EnemyGenerateSetting> spawnHelper;
        public SpawnerType spawnerType => SpawnerType.Enemy;
        List<IEnemy> currentEnemys = new List<IEnemy>();
        public UnityAction<bool> OnChangeCount { get; set; }
        public bool IsSetUped { get; set; } = false;

        public ISpawnableObj GetTargetObj()
        {
            var prefabSetting = spawnHelper.prefabGenerateSetting;
            var enemyLength = prefabSetting.EnemyPrefabs.Count;
            var r = UnityEngine.Random.Range(0, enemyLength);
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
            if (!enemy.TryGetComponent<IEnemy>(out var enemyInterface)) return;

            currentEnemys.Add(enemyInterface);
            OnChangeCount?.Invoke(false);
            enemyInterface.OnDeadAction += (enemy) =>
            {
                RemoveEnemy(enemy);
                OnChangeCount?.Invoke(true);
            };
            spawnHelper.occupyMap[targetX, targetY] = 1;
        }     

        void RemoveEnemy(IEnemy enemy) => currentEnemys.Remove(enemy);
        // Update is called once per frame
        void Update()
        {
            if (spawnHelper == null || BattleStateManager.Instance.isEndBattle) return;
            if (spawnHelper.IsReachedSpawnTime() && currentEnemys.Count < spawnableCount
                && IsSetUped)
            {
                if (spawnHelper.IsSpawnable(out var node)) Spawn(node.x, node.y);
            }
        }
    }
}


