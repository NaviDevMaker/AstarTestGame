using UnityEngine;


namespace Game.Spawner
{
    public interface ISpawnPrefab { }
    public class SpawnDataBase : ScriptableObject,ISpawnPrefab
    {
        [SerializeField] float spawnTime; 
        public float SpawnTime  => spawnTime;
    }
}

