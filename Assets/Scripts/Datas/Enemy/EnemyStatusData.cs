using UnityEngine;

[CreateAssetMenu]
public class EnemyStatusData : StatusData
{
    [SerializeField] int damageAmount;
    [SerializeField] int score;
    public int DamageAmount  => damageAmount;
    public int Score  => score;
}
