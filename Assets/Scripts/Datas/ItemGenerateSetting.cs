using UnityEngine;
using System.Collections.Generic;
using Game.SpawnableObj;
using System.Linq;
namespace Game.Spawner
{
    [CreateAssetMenu]
    public class ItemGenerateSetting : SpawnDataBase
    {
        //Monobevieâ‘Î‚¾‚ß‚Ë
        [Header("Item Prefabs")]
        [SerializeField] List<GameObject> prefabs;

        //List ê—p
        //•ÏŠ·Œã‚à List ‚Ì‚Ü‚Ü•Ô‚¹‚é
        public List<ISpawnableObj> Prefabs
              => prefabs.Where(p => p.GetComponent<ISpawnableObj>() != null)
                 .Select(s => s.GetComponent<ISpawnableObj>())
                 .ToList();
    }
}

