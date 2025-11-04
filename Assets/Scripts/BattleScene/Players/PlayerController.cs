using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using System;
namespace Game.Player
{
    public interface IPlayer<TPlayer> where TPlayer : MonoBehaviour, IPlayer<TPlayer>
    {
        (int hash,string clipName) GetAnimInfo(PlayerStateMachineBase<TPlayer> stateMachineBase);
    }
    public class PlayerController : MonoBehaviour,IAssetSetter,IPlayer<PlayerController>
    {
        public AnimationData animationData { get; private set; }
        public PlayerIdleState _playerIdleState { get; private set;}
        public PlayerWalkState _playerWalkState { get; private set;}
        public PlayerAttackState _playerAttackState { get; private set; }
        public PlayerDeathState _playerDeathState { get; private set;}

        PlayerStateMachineBase<PlayerController> currentState = null;

        [SerializeField] PlayerStatusData statusData;
        public PlayerStatusData playerStatusData => statusData;
        public Animator animator { get; private set; }
        public bool isDead { get; private set; }
        public IEnemy currentTarget { get; set;}

        public UnityAction OnHitEnemy;
        public UnityAction OnRestoreLife;
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
            if (InputManager.AttackButtonPressed()) _playerAttackState.Attack();
            currentState?.OnUpdate();
        }
        private void LateUpdate()
        {
            
        }
        void Initialize()
        {
            animator = GetComponentInChildren<Animator>();
            StateSetup();
            ChangeState(_playerIdleState);
        }
        void StateSetup()
        {
            _playerIdleState = new PlayerIdleState(this);
            _playerWalkState = new PlayerWalkState(this);
            _playerAttackState = new PlayerAttackState(this);
            _playerDeathState = new PlayerDeathState(this);
        }
        bool CanPickUpItem()
        {
            var targetPos = _playerWalkState.perTargetPos;
            var forward = transform.forward;
            var toTarget = (targetPos - transform.position).normalized;
            forward.y = 0f;
            toTarget.y = 0f;
            var dot = Vector3.Dot(forward, toTarget);
            var pickUpAngle = playerStatusData.PickUpAngle;
            var thereHold = Mathf.Cos(pickUpAngle * 0.5f * Mathf.Deg2Rad);
            return dot >= thereHold;
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
                PlayerDeathState => (animationData.DeathHash,animationData.DeathClipName),
                _ => default
            };
        }
    }
}


