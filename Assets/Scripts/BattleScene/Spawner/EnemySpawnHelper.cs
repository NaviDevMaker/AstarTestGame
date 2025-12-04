using Cysharp.Threading.Tasks;
using Game.Spawner;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
    public class EnemySpawnHelper:SpawnHelper<EnemyGenerateSetting>
    {
        public EnemySpawnHelper(SpawnerType spawnerType) : base(spawnerType) { }
        enum Round
        {
            Start,
            Two,
            Three,
            Four,
            End
        }

        Round currentRound;
        Dictionary<Round, double> timeDic = new Dictionary<Round, double>() { };
        public bool IsSpawnableTimeAndCount(List<IEnemy> currentEnemys) =>
                         IsReachedSpawnTime() && currentEnemys.Count < prefabGenerateSetting.SpawnableCount;

        double nextSpawnTime = 0f;
        float roundEndTime = 0f;
        bool dictionalySetUped = false;
        void DictionaySet()
        {
            timeDic[Round.Start] = TimeSpan.FromMinutes(prefabGenerateSetting.ToTwoRound).TotalSeconds;
            timeDic[Round.Two] = TimeSpan.FromMinutes(prefabGenerateSetting.ToThreeRound).TotalSeconds;
            timeDic[Round.Three] = TimeSpan.FromMinutes(prefabGenerateSetting.ToFourRound).TotalSeconds;
            timeDic[Round.Four] = TimeSpan.FromMinutes(prefabGenerateSetting.ToFiveRound).TotalSeconds;
            dictionalySetUped = true;
        }

        protected override async UniTask Initialize(SpawnerType spawnerType)
        {
            await base.Initialize(spawnerType);
            DictionaySet();
            currentRound = Round.Start;
            nextSpawnTime = timeDic[currentRound];
        }
        public void ChangeSpawnTime()
        {
            if (currentRound == Round.End || !dictionalySetUped) return;
            if((GameTimer.Instance.elapsedGameTime - roundEndTime) >= nextSpawnTime)
            {
                AudioManager.Instance.PlayCountDownSE(true);
                spawnTime--;
                currentRound = currentRound switch
                {
                    Round.Start => Round.Two,
                    Round.Two => Round.Three,
                    Round.Three => Round.Four,
                    Round.Four => Round.End,
                    _ => Round.End,
                };
                if (currentRound == Round.End) return;
                nextSpawnTime = timeDic[currentRound];
                roundEndTime = GameTimer.Instance.elapsedGameTime;
            }
        }
    }
}

