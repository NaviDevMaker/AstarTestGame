using UnityEngine;

public class StateMachine
{
    StateBase currentState;
    public EnemyIdleStateBase IdleState { get; private set;}
    public EnemyMoveStateBase MoveState { get; private set;}

    public EnemyDeathStateBase DeathState { get; private set;}

    GameObject owner;
    Animator animator;
    public StateMachine(GameObject owner,Animator animator,
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

    public void Update() => currentState?.OnUpdate();
}
