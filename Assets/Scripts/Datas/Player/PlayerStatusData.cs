using UnityEngine;

[CreateAssetMenu]
public class PlayerStatusData :StatusData
{
    [SerializeField] int life;
    [Header("Action Infomation")]
    [SerializeField] float pickUpAngle;
    [SerializeField] float pickUpRadius;
    [SerializeField] float rotateSpeed;
    [SerializeField] float detectRange;
    [SerializeField] int attackEndFrame;
    public int Life => life;
    public float RotateSpeed => rotateSpeed;

    public float DetectRange  => detectRange;

    public int AttackEndFrame => attackEndFrame;

    public float PickUpAngle  => pickUpAngle;

    public float PickUpRadius  => pickUpRadius;
} 
