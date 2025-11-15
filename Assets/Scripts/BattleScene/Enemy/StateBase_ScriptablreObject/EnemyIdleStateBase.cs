using UnityEngine;

[CreateAssetMenu]
public class EnemyIdleStateBase : StateBase
{
    public override void Initialize(StateMachine stateMachine, IEnemy owner, Animator animator)
    {
        base.Initialize(stateMachine, owner, animator);
        animatorHash = Animator.StringToHash("isIdling");
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnEnterChangeAnimation() => animator.SetBool(animatorHash,true);
    public override void OnExitChangeAnimation() => animator.SetBool(animatorHash,false);

    public override void OnUpdate()
    {
    }
}
