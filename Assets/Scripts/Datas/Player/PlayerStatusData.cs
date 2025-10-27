using UnityEngine;

[CreateAssetMenu]
public class PlayerStatusData :StatusData
{
    [SerializeField] int life;
    [SerializeField] float rotateSpeed;
    public int Life => life;
    public float RotateSpeed => rotateSpeed;
} 
