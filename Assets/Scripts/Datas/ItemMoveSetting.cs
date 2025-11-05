using UnityEngine;

[CreateAssetMenu]
public class ItemMoveSetting : ScriptableObject
{
    [SerializeField] float moveSpeed;
    [Header("Target Offset")]
    [SerializeField] float moveForwardOffset;
    [SerializeField] float upOffset;
    [SerializeField] float downOffset;

    public float MoveSpeed => moveSpeed;
    public float MoveForwardOffset => moveForwardOffset;
    public float UpOffset => upOffset;
    public float DownOffset => downOffset;
}
public enum MoveInfo
{
    MoveFoward,
    Up,
    down
}
