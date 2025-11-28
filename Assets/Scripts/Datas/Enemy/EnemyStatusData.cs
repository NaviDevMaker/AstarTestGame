using UnityEngine;

[CreateAssetMenu]
public class EnemyStatusData : StatusData
{
    [SerializeField] int damageAmount;
    [SerializeField] int score;
    [Header("Visiblity distance")]
    [SerializeField] float visibleDistBasedSqr;
    public int DamageAmount  => damageAmount;
    public int Score  => score;
    public float VisibleDistBasedSqr => visibleDistBasedSqr;
}
