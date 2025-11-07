using UnityEngine;
using Game.Player;
using System.Linq;
using System.Reflection;
public class EnemyHitJudgement : MonoBehaviour
{
    EnemyController owner;
    int damage = 0;
    private void Awake()
    {
        owner = transform.parent.GetComponent<EnemyController>();
        damage = owner.EnemyStatusData.DamageAmount;
    }

    private void OnTriggerStay(Collider other)
    {
        var obj = other.gameObject;
        DamageProvider.TryAddDamage(obj, damage);
    }
}
