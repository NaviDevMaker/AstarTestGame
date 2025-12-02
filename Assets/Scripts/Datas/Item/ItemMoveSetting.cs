using UnityEngine;

[CreateAssetMenu]
public class ItemMoveSetting : ScriptableObject
{
    [Header("Speed")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float floatSpeed;

    [Header("Target Offset")]
    [SerializeField] float upOffset;
    [SerializeField] float furtherUpOffset;
    [SerializeField] float downOffset;
    [SerializeField] float floatOffset;
    [Header("Duration")]
    [SerializeField] float rotateDuration;

    public float MoveSpeed => moveSpeed;
    public float UpOffset => upOffset;
    public float DownOffset => downOffset;

    public float FurtherUpOffset  => furtherUpOffset;

    public float RotateSpeed  => rotateSpeed;

    public float RotateDuration => rotateDuration;

    public float FloatSpeed => floatSpeed;

    public float FloatOffset => floatOffset;
}
public enum MoveInfo
{
    MoveFoward,
    Up,
    Down
}
