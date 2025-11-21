using UnityEngine;
using Game.Player;
using System.Linq;
using System.Reflection;

namespace Game.Enemy
{
    public class EnemyHitJudgement : MonoBehaviour
    {
        EnemyController owner;
        int damage = 0;
        private void Awake()
        {
            owner = transform.parent.GetComponent<EnemyController>();
            damage = owner._enemyStatusData.DamageAmount;
        }

        private void OnTriggerStay(Collider other)
        {
            if (owner.isDead) return;
            Debug.Log("adafdjadfhaeljfdalsjda");
            var obj = other.gameObject;
            DamageProvider.TryAddDamage(obj, damage);
        }
    }

}
