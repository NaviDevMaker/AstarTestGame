using UnityEngine;

public class StateMachine
{
    public StateBase currentState { get; private set;}
    public EnemyIdleStateBase IdleState { get; private set;}
    public EnemyMoveStateBase MoveState { get; private set;}

    public EnemyDeathStateBase DeathState { get; private set;}

    IEnemy owner;
    Animator animator;
    public StateMachine(IEnemy owner,Animator animator,
                      EnemyIdleStateBase idleState,EnemyMoveStateBase moveState,EnemyDeathStateBase deathState)
    {
        this.owner = owner;
        this.animator = animator;
        this.IdleState = idleState;
        this.MoveState = moveState;
        this.DeathState = deathState;

        IdleState.Initialize(this,owner,animator);
        MoveState.Initialize(this, owner,animator);
        DeathState.Initialize(this, owner,animator);
    }

    public void ChangeState(StateBase nextState)
    {
        currentState?.OnExit();
        currentState = nextState;
        currentState?.OnEnter();
    }
    public void ChangeToDeathState()
    {
        //if (currentState == DeathState) return;
        Debug.Log("Ž€‹Ž", owner.owerObj);
        ChangeState(DeathState);
    }
    public void Update() => currentState?.OnUpdate();
}
