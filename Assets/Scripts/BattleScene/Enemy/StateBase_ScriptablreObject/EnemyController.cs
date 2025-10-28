using UnityEngine;
using UnityEngine.Events;


public interface IEnemy 
{
    UnityAction OnDeadAction { get; }
}

[RequireComponent(typeof(Animator))]

public class EnemyController : MonoBehaviour,IEnemy
{
    [SerializeField] EnemyIdleStateBase idleState;
    [SerializeField] EnemyMoveStateBase moveState;
    [SerializeField] EnemyDeathStateBase deathState;

    StateMachine stateMachine;

    public UnityAction OnDeadAction { get; private set;}

    void Start()
    {
        Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
    void Initialize()
    {
        var animator = GetComponent<Animator>();
        stateMachine = new StateMachine(this.gameObject, animator,idleState, moveState, deathState);
        OnDeadAction = stateMachine.ChangeToDeathState;
        stateMachine.ChangeState(idleState);
    }
}
