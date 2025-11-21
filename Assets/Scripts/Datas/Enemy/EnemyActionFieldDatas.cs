using UnityEngine;

namespace Game.Enemy
{
    [CreateAssetMenu]
    public class EnemyActionFieldDatas : ScriptableObject
    {
        [Header("Translusent alpha change")]
        [SerializeField] float changeSpeed;
        public float ChangeSpeed => changeSpeed;
    }
}


