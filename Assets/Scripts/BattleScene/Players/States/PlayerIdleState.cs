using UnityEngine;

namespace Game.Player
{
    public class PlayerIdleState : StateMachineBase<PlayerController>
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
            controller.ChangeState(controller.PlayerMoveState);
        }
    }
}

