using UnityEngine;
using System.Collections.Generic;

namespace Game.Item
{
    [CreateAssetMenu]
    public class ItemGenerateSetting : ScriptableObject
    {
        [Header("Item Prefabs")]
        [SerializeField] List<GameObject> prefabs;

        public List<GameObject> Prefabs  => prefabs;
    }
}

