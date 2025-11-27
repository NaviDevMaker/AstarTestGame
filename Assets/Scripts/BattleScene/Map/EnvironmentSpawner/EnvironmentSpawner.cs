using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
namespace Game.Stage
{
    public class EnvironmentSpawnerInfo
    {
       public int mapSizeW {get; set;}
       public int mapSizeH {get; set;}
       public Vector3 defaultPos {get; set;}
       
       public Terrain terrain {get; set;}
    }
    public interface IEnvironmentSpawner
    {
       void SpawnObjectAroundStage(EnvironmentSpawnerInfo environmentSpawnerInfo);
    }

    public class EnvironmentSpawner : MonoBehaviour
    {
        [SerializeField] List<EnvironmentSpawnerBase> spawners;

        public void SpawnAll(EnvironmentSpawnerInfo environmentSpawnerInfo)
        => spawners.ForEach(spawner => spawner.SpawnObjectAroundStage(environmentSpawnerInfo));
    }
}


