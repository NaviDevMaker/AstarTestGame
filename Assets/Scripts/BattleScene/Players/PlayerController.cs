using Cysharp.Threading.Tasks;
using UnityEngine;
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

        public PlayerStatusData playerStatusData { get; private set;}
        public Animator animator { get; private set; }

        public bool isDead { get; private set; }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await GetAsset();
            Initialize();
        }
        // Update is called once per frame
        void Update()
        {
            if (InputManager.AttackButtonPressed()) _playerAttackState.Attack();
            currentState?.OnUpdate();
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
        public void ChangeState(PlayerStateMachineBase<PlayerController> nextState)
        {
            currentState?.OnExit();
            currentState = nextState;
            currentState.OnEnter();
        }
        public async UniTask GetAsset()
        {
            var statusData = await GetAssetsMethods.GetAsset<PlayerStatusData>("Datas/PlayerStatusData");
            var animData = await GetAssetsMethods.GetAsset<AnimationData>("Datas/PlayerAnimationData");
            if (statusData == null || animData == null) throw new System.Exception();
            playerStatusData = statusData;
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


