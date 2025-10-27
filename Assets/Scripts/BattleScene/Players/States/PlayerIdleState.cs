using UnityEngine;

namespace Game.Player
{
    public class PlayerIdleState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerIdleState(PlayerController controller) : base(controller) { }
        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
           var key = InputManager.GetKey();
           if(key != PressedKey.None)  controller.ChangeState(controller._playerWalkState);
        }

        public override void Initialize() { }
    }
}

