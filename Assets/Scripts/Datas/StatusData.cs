using UnityEngine;

[CreateAssetMenu]
public class StatusData : ScriptableObject
{
    [SerializeField] float moveSpeed;
    public  float MoveSpeed  => moveSpeed;
}
