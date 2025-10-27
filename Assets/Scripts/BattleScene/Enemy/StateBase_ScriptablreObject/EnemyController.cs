using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyIdleStateBase idleState;
    [SerializeField] EnemyMoveStateBase moveState;
    [SerializeField] EnemyDeathStateBase deathState;

    StateMachine stateMachine;
  
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
        stateMachine.ChangeState(idleState);
    }
}
