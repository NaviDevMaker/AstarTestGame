using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Game.Item;
using System.Linq;



namespace Game.Player
{
    public interface IPlayer<TPlayer> : IDamageable where TPlayer : MonoBehaviour, IPlayer<TPlayer>
    {
        (int hash,string clipName) GetAnimInfo(PlayerStateMachineBase<TPlayer> stateMachineBase);
        UnityAction OnHitEnemyAction { get; set;}
        UnityAction OnDeadAction { get; set;}
        bool isDead { get;}
        bool isInvincible { get; set;}
    }
    public class PlayerController : MonoBehaviour,IAssetSetter,IPlayer<PlayerController>,IDamageable
    {
        public AnimationData animationData { get; private set; }
        public PlayerIdleState _playerIdleState { get; private set;}
        public PlayerWalkState _playerWalkState { get; private set;}
        public PlayerAttackState _playerAttackState { get; private set; }
        public PlayerHitState _playerHitState { get; private set; }
        public PlayerDeathState _playerDeathState { get; private set;}

        public PlayerItemPickUpState _playerItemPickUpState { get; private set;}
        PlayerStateMachineBase<PlayerController> currentState = null;

        [SerializeField] PlayerStatusData statusData;
        public PlayerStatusData playerStatusData => statusData;
        public Animator animator { get; private set; }
        public bool isDead { get; private set; }
        public IEnemy currentTarget { get; set;}
        public UnityAction OnHitEnemyAction { get; set;}
        public int currentLife { get; set;}
        public bool isInvincible { get; set;}
        public UnityAction OnDeadAction { get; set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await GetAsset();
            Initialize();
        }
        // Update is called once per frame
        void Update()
        {
            Debug.Log(currentTarget);
            if (isDead && currentState != _playerDeathState)
            {
                DeathAction();
                return;
            }
            if (isDead) return;
            if (InputManager.AttackButtonPressed()) _playerAttackState.Attack().Forget();
            if (InputManager.PickUpItemButtonPressed()) _playerItemPickUpState.TryPickUpItem();
            currentState?.OnUpdate();
        }
        void Initialize()
        {
            animator = GetComponentInChildren<Animator>();
            StateSetup();
            PlayerSetUp();
            ChangeState(_playerIdleState);
        }
        void StateSetup()
        {
            _playerIdleState = new PlayerIdleState(this);
            _playerWalkState = new PlayerWalkState(this);
            _playerAttackState = new PlayerAttackState(this);
            _playerHitState = new PlayerHitState(this);
            _playerDeathState = new PlayerDeathState(this);
            _playerItemPickUpState = new PlayerItemPickUpState(this);
        }
        void PlayerSetUp()
        {
            currentLife = playerStatusData.Life;
            OnHitEnemyAction += _playerHitState.WaitInvincibleTime;       
        }
        public void ChangeState(PlayerStateMachineBase<PlayerController> nextState)
        {
            currentState?.OnExit();
            currentState = nextState;
            currentState.OnEnter();
        }
        public async UniTask GetAsset()
        {
            var animData = await GetAssetsMethods.GetAsset<AnimationData>("Datas/PlayerAnimationData");
            if (statusData == null || animData == null) throw new System.Exception();
            animationData = animData;
        }
        public (int hash,string clipName) GetAnimInfo(PlayerStateMachineBase<PlayerController> stateMachineBase)
        {
            return stateMachineBase switch
            {
                PlayerWalkState => (animationData.WalkHash,animationData.WalkClipName),
                PlayerAttackState => (animationData.AttackHash,animationData.AttackClipName),
                PlayerItemPickUpState => (animationData.PickUpHash,animationData.PickUpClipName),
                PlayerDeathState => (animationData.DeathHash,animationData.DeathClipName),
                _ => default
            };
        }
        void DeathAction()
        {
            ChangeState(_playerDeathState);
            OnDeadAction?.Invoke();
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, statusData.PickUpRadius);
        }
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            currentLife -= damage;
            LifeManager.Instance.ReduceLife();
            if(currentLife <= 0) isDead = true;
        }
        public void SetHashToFalse() => _playerItemPickUpState.SetHashToFalse();
    }
}


