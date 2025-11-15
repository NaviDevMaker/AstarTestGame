using UnityEngine;

[CreateAssetMenu]
public class EnemyDeathStateBase : StateBase
{
    public override void OnEnterChangeAnimation() => animator.SetTrigger(animatorHash);
    public override void Initialize(StateMachine stateMachine, IEnemy owner, Animator animator)
    {
        base.Initialize(stateMachine, owner, animator);
        animatorHash = Animator.StringToHash("isDead");
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log($"Ž€–S,{owner.owerObj.name}");
        //UnityEngine.Object.Destroy(owner.owerObj);
    }
    public override void OnExit()
    {
        base.OnExit();
    }
    public override void OnUpdate()
    {
    }
    public override void OnExitChangeAnimation(){ }
}
