using UnityEngine;

namespace Game.Player
{
    public class PlayerDeathState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerDeathState(PlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.animator.SetTrigger(animatorHash);
        }

        public override void OnExit() { }
        public override void OnUpdate() { }
    }
}

