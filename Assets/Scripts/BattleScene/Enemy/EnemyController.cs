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
        UnityAction<IEnemy> OnDeadAction { get; set;}
        EnemyStatusData _enemyStatusData { get; }
        bool isDead { get; set; }
        Material meshMat { get; }
        void StateMachineSet();
        Collider enemyCollider { get; }
        EnemyActionHelper<EnemyController> enemyActionHelper { get; }

        EnemyAudioHelper<EnemyController> enemyAudioHelper { get; }
    }
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class EnemyController : MonoBehaviour, IEnemy, ISpawnableObj
    {
        [SerializeField] EnemyStatusData enemyStatusData;
        [SerializeField] EnemyAudioDatas enemyAudioDatas;

        [SerializeField] AudioSource audioSource;
        [SerializeField] EnemyIdleStateBase idleStateTemplate;
        [SerializeField] EnemyMoveStateBase moveStateTemplate;
        [SerializeField] EnemyDeathStateBase deathStateTemplate;

        StateMachine stateMachine;
        public UnityAction<IEnemy> OnDeadAction { get; set; }
        public GameObject owerObj => this.gameObject;
        public Collider enemyCollider { get; private set; }
        public EnemyStatusData _enemyStatusData => enemyStatusData;
        public GameObject ownerObj => gameObject;
        public Material meshMat { get; private set; }
        public bool isDead { get; set; } = false;
        public EnemyActionHelper<EnemyController> enemyActionHelper { get; private set;}
        public EnemyAudioHelper<EnemyController> enemyAudioHelper { get; private set; }
        void Start()
        {
            Initialize();
            enemyActionHelper?.StartTranslusentAction().Forget();
        }
        // Update is called once per frame
        void Update()
        {
            stateMachine?.Update();
            if (isDead) return;
            TargetManager.Instance.SetCurrentTarget(this);
            enemyActionHelper?.ChangeVisible();
        }
        void Initialize()
        {
            enemyCollider = GetComponentInChildren<Collider>();
            var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            meshMat = renderer.material;
            enemyActionHelper = new EnemyActionHelper<EnemyController>(this);
            StateMachineSet();
            AudioSetup();
        }
        void AudioSetup()
        {
            enemyAudioHelper = new EnemyAudioHelper<EnemyController>(this, audioSource, enemyAudioDatas);
            enemyAudioHelper.PlayMoveAudio();
        }
        public void StateMachineSet()
        {
            var animator = GetComponent<Animator>();
            var idle = idleStateTemplate.Clone<EnemyIdleStateBase>();
            var move = moveStateTemplate.Clone<EnemyMoveStateBase>();
            var death = deathStateTemplate.Clone<EnemyDeathStateBase>();
            stateMachine = new StateMachine(this, animator, idle, move, death);
            OnDeadAction += (_) => stateMachine.ChangeToDeathState();
            stateMachine.ChangeState(idle);
        }
    }
}

