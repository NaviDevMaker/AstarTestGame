using UnityEngine;

namespace Game.Stage
{
    public abstract class EnvironmentSpawnerBase : MonoBehaviour, IEnvironmentSpawner
    {
        public abstract void SpawnObjectAroundStage(EnvironmentSpawnerInfo environmentSpawnerInfo);
    }
}


