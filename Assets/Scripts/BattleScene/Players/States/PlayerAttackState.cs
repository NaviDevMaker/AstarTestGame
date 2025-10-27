using UnityEngine;

namespace Game.Player
{
    public class PlayerAttackState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerAttackState(PlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public override void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}

