using UnityEngine;

[CreateAssetMenu]
public class PlayerStatusData :StatusData
{
    [SerializeField] int life;
    [Header("Action Infomation")]
    [SerializeField] float attackAngle;
    [SerializeField] float pickUpAngle;
    [SerializeField] float pickUpRadius;
    [SerializeField] float rotateSpeed;
    [SerializeField] float detectRange;
    [SerializeField] int attackEndFrame;
    [SerializeField] float invincibleDuration;
    public int Life => life;
    public float RotateSpeed => rotateSpeed;

    public float DetectRange  => detectRange;

    public int AttackEndFrame => attackEndFrame;

    public float PickUpAngle  => pickUpAngle;

    public float PickUpRadius  => pickUpRadius;

    public float AttackAngle  => attackAngle;
    public float InvincibleDuration => invincibleDuration; 
} 
