using UnityEngine;
using System.Collections.Generic;
using Game.Enemy;
namespace Game.Spawner
{
    [CreateAssetMenu]
    public class EnemyGenerateSetting : SpawnDataBase
    { 
        [Header("Enemy Prefabs")]
        [SerializeField] List<EnemyController> enemyPrefabs;

        [Header("Game Status")]
        [SerializeField] int spawnableCount;
        [SerializeField] float toTwoRoundTime;
        [SerializeField] float toThreeRoundTime;
        [SerializeField] float toFourRoundTime;
        [SerializeField] float toFiveRoundTime;
        public List<EnemyController> EnemyPrefabs => enemyPrefabs;
        public int SpawnableCount => spawnableCount;

        public float ToTwoRound  => toTwoRoundTime; 
        public float ToThreeRound  => toThreeRoundTime;
        public float ToFourRound => toFourRoundTime; 
        public float ToFiveRound => toFiveRoundTime; 
    }
}


