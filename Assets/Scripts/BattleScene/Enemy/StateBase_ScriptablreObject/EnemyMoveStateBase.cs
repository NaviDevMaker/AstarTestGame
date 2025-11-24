using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Game.Enemy
{
    [CreateAssetMenu]
    public class EnemyMoveStateBase : StateBase
    {
        CancellationTokenSource chaseEndCts;
        public override void Initialize(StateMachine stateMachine, IEnemy owner, Animator animator)
        {
            base.Initialize(stateMachine, owner, animator);
            animatorHash = Animator.StringToHash("isMoving");
        }
        public override void OnEnter()
        {
            chaseEndCts = new CancellationTokenSource();
            owner.enemyActionHelper.ChaseLooper(chaseEndCts).Forget();
            base.OnEnter();
        }


        public override void OnExit()
        {
            chaseEndCts?.Cancel();
            chaseEndCts?.Dispose();
            base.OnExit();
        }
        public override void OnEnterChangeAnimation() => animator.SetBool(animatorHash, true);
        public override void OnExitChangeAnimation() => animator.SetBool(animatorHash, false);
        public override void OnUpdate()
        {
            
        }
    }

}

