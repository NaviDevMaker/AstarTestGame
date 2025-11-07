using UnityEngine;
using UnityEngine.Events;
using Game.Player;

public interface IEnemy 
{
    GameObject owerObj { get; }
    UnityAction OnDeadAction { get; }

    Collider enemyCollider { get;}
}

[RequireComponent(typeof(Animator))]

public class EnemyController : MonoBehaviour, IEnemy
{
    [SerializeField] EnemyStatusData enemyStatusData;
    [SerializeField] EnemyIdleStateBase idleState;
    [SerializeField] EnemyMoveStateBase moveState;
    [SerializeField] EnemyDeathStateBase deathState;

    StateMachine stateMachine;
    public UnityAction OnDeadAction { get; private set; }

    public GameObject owerObj => this.gameObject;

    public Collider enemyCollider { get; private set;}
    public EnemyStatusData EnemyStatusData  => enemyStatusData;

    void Start()
    {
        Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        TargetManager.Instance.SetCurrentTarget(this);
    }
    void Initialize()
    {
        var animator = GetComponent<Animator>();
        enemyCollider = GetComponentInChildren<Collider>();
        stateMachine = new StateMachine(this, animator, idleState, moveState, deathState);
        OnDeadAction = stateMachine.ChangeToDeathState;
        stateMachine.ChangeState(idleState);
    }
}
