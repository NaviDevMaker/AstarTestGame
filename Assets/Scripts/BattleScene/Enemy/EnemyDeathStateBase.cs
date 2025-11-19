using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
namespace Game.Enemy
{
    [CreateAssetMenu]
    public class EnemyDeathStateBase : StateBase
    {
        public override void OnEnterChangeAnimation() => animator.SetTrigger(animatorHash);
        public override void Initialize(StateMachine stateMachine, IEnemy owner, Animator animator)
        {
            base.Initialize(stateMachine, owner, animator);
            animatorHash = Animator.StringToHash("isDead");
        }
        public override void OnEnter()
        {
            base.OnEnter();
            WaitDeadAction().Forget();
            Debug.Log($"Ž€–S,{owner.owerObj.name}");
            //UnityEngine.Object.Destroy(owner.owerObj);
        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override void OnUpdate()
        {
        }
        public override void OnExitChangeAnimation() { }

        async UniTask WaitDeadAction()
        {
            try
            {
                await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Death")
                                        , cancellationToken: owner.owerObj.GetCancellationTokenOnDestroy());
                await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f
                                        , cancellationToken: owner.owerObj.GetCancellationTokenOnDestroy());
                UnityEngine.Object.Destroy(owner.owerObj);
            }
            catch (OperationCanceledException) { }
        }
    }

}

