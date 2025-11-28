using UnityEngine;

namespace Game.Enemy
{
    [CreateAssetMenu]
    public class EnemyActionFieldDatas : ScriptableObject
    {
        [Header("Translusent alpha change")]
        [SerializeField] float changeSpeed;

        [Header("Death Move up Action")]
        [SerializeField] float upAmount;
        [SerializeField] float upDuration;

        [Header(" A* get new pathes distance")]
        [SerializeField] float distBasedSqr;

        public float ChangeSpeed => changeSpeed;
        public float UpAmount => upAmount;
        public float UpDuration  => upDuration;
        public float DistBasedSqr  => distBasedSqr;
    }
}


