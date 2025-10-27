using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
namespace Game.Player
{
    public interface IPlayer<TPlayer> where TPlayer : MonoBehaviour, IPlayer<TPlayer>
    {
        int GetHash(PlayerStateMachineBase<TPlayer> stateMachineBase);
    }
    public class PlayerController : MonoBehaviour,IAssetSetter,IPlayer<PlayerController>
    {
        public AnimatorHash animatorHash { get; private set; }
        public PlayerIdleState _playerIdleState { get; private set;}
        public PlayerWalkState _playerWalkState { get; private set;}
        public PlayerAttackState _playerAttackState { get; private set; }
        public PlayerDeathState _playerDeathState { get; private set;}

        PlayerStateMachineBase<PlayerController> currentState = null;

        public PlayerStatusData playerStatusData { get; private set;}
        public Animator animator { get; private set; }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await GetAsset();
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
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
            var hashData = await GetAssetsMethods.GetAsset<AnimatorHash>("Datas/PlayerAnimatorHash");
            if (statusData == null || hashData == null) throw new System.Exception();
            playerStatusData = statusData;
            animatorHash = hashData;
        }

        public int GetHash(PlayerStateMachineBase<PlayerController> stateMachineBase)
        {
            return stateMachineBase switch
            {
                PlayerWalkState => animatorHash.walkHash,
                PlayerAttackState => animatorHash.attackHash,
                PlayerDeathState => animatorHash.deathHash,
                _ => default
            };
        }
    }
}


