using UnityEngine;
using UnityEngine.Events;
using Game.Player;
using Game.SpawnableObj;
using Cysharp.Threading.Tasks;

namespace Game.Enemy
{
    public interface IEnemy
    {
        GameObject owerObj { get; }
        UnityAction OnDeadAction { get; }

        bool isDead { get; set;}
        Material meshMat { get; }
        void StateMachineSet();
        Collider enemyCollider { get; }
    }

    [RequireComponent(typeof(Animator))]
    public class EnemyController : MonoBehaviour, IEnemy, ISpawnableObj
    {
        [SerializeField] EnemyStatusData enemyStatusData;
        [SerializeField] EnemyIdleStateBase idleStateTemplate;
        [SerializeField] EnemyMoveStateBase moveStateTemplate;
        [SerializeField] EnemyDeathStateBase deathStateTemplate;

        StateMachine stateMachine;
        public UnityAction OnDeadAction { get; private set; }
        public GameObject owerObj => this.gameObject;
        public Collider enemyCollider { get; private set; }
        public EnemyStatusData EnemyStatusData => enemyStatusData;
        public GameObject ownerObj => gameObject;
        public Material meshMat { get; private set; }
        public bool isDead { get; set; } = false;

        EnemyActionHelper<EnemyController> enemyActionHelper;
        void Start()
        {
            Initialize();
        }
        // Update is called once per frame
        void Update()
        {
            stateMachine?.Update();
            enemyActionHelper?.StartTranslusentAction().Forget();
            TargetManager.Instance.SetCurrentTarget(this);
        }
        void Initialize()
        {
            enemyCollider = GetComponentInChildren<Collider>();
            var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            meshMat = renderer.material;
            StateMachineSet();
            enemyActionHelper = new EnemyActionHelper<EnemyController>(this);
        }
        public void StateMachineSet()
        {
            var animator = GetComponent<Animator>();
            var idle = idleStateTemplate.Clone<EnemyIdleStateBase>();
            var move = moveStateTemplate.Clone<EnemyMoveStateBase>();
            var death = deathStateTemplate.Clone<EnemyDeathStateBase>();
            stateMachine = new StateMachine(this, animator, idle, move, death);
            OnDeadAction = stateMachine.ChangeToDeathState;
            stateMachine.ChangeState(idle);
        }
    }
}

