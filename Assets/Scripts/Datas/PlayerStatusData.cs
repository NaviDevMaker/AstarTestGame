using UnityEngine;

[CreateAssetMenu]
public class PlayerStatusData :StatusData
{
    [SerializeField] int life;

    public int Life => life;
} 
