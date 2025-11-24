using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


namespace Game.Enemy
{
    [CreateAssetMenu]
    public class EnemyIdleStateBase : StateBase
    {
        public override void Initialize(StateMachine stateMachine, IEnemy owner, Animator animator)
        {
            base.Initialize(stateMachine, owner, animator);
            nextState = stateMachine.MoveState;
            animatorHash = Animator.StringToHash("isIdling");
        }
        public override async void OnEnter()
        {
            base.OnEnter();
            try
            {
                await owner.enemyActionHelper.SpawnAction();
                stateMachine.ChangeState(nextState);
            }
            catch (OperationCanceledException) { }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnEnterChangeAnimation() => animator.SetBool(animatorHash, true);
        public override void OnExitChangeAnimation() => animator.SetBool(animatorHash, false);

        public override void OnUpdate()
        {
        }

        
    }

}

