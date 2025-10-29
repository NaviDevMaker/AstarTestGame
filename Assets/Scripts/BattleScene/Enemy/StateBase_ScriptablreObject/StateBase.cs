using UnityEngine;

public abstract class StateBase : ScriptableObject
{
    protected StateMachine stateMachine;
    protected IEnemy owner;
    protected Animator animator;
    public virtual void Initialize(StateMachine stateMachine,IEnemy owner,Animator animator)
    {
        this.stateMachine = stateMachine;
        this.owner = owner;
        this.animator = animator;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
