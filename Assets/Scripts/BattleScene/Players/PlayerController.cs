using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour,IAssetSetter
    {
        public PlayerIdleState PlayerIdleState { get; private set;}
        public PlayerMoveState PlayerMoveState { get; private set;}
        public PlayerAttackState PlayerAttackState { get; private set; }
        public PlayerDeathState PlayerDeathState { get; private set;}

        StateMachineBase<PlayerController> currentState = null;

        public PlayerStatusData PlayerStatusData { get; private set;}
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
            PlayerIdleState = new PlayerIdleState(this);
            PlayerMoveState = new PlayerMoveState(this);
            PlayerAttackState = new PlayerAttackState(this);
            PlayerDeathState = new PlayerDeathState(this);
            ChangeState(PlayerIdleState);
        }
        public void ChangeState(StateMachineBase<PlayerController> nextState)
        {
            currentState?.OnExit();
            currentState = nextState;
            currentState.OnEnter();
        }
        public async UniTask GetAsset()
        {
            var data = await GetAssetsMethods.GetAsset<PlayerStatusData>("Datas/PlayerStatusData");
            if (data == null) throw new System.Exception();
            PlayerStatusData = data;
        }
    }
}


