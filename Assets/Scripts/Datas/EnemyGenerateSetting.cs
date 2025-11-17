using UnityEngine;
using System.Collections.Generic;

namespace Game.Spawner
{
    [CreateAssetMenu]
    public class EnemyGenerateSetting : SpawnDataBase
    { 
        [Header("Enemy Prefabs")]
        [SerializeField] List<EnemyController> enemyPrefabs;
        public List<EnemyController> EnemyPrefabs => enemyPrefabs;
    }
}


