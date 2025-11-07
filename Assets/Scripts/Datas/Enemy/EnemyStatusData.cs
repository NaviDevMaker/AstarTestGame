using UnityEngine;

[CreateAssetMenu]
public class EnemyStatusData : StatusData
{
    [SerializeField] int damageAmount;

    public int DamageAmount  => damageAmount;
}
