using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
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
            token = owner.owerObj.GetCancellationTokenOnDestroy();
        }
        CancellationToken token;
        public override void OnEnter()
        {
            base.OnEnter();
            WaitDeadAction().Forget();
            Debug.Log($"Ž€–S,{owner.owerObj.name}");
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
                var actionHelper = owner.enemyActionHelper;
                await actionHelper.MoveUpAction();
                await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Death")
                                        , cancellationToken:token);
                var animationLength = GetStateAnimationLength();
                owner.enemyAudioHelper.PlayDeathAudio(animationLength);
                var fadeTask = actionHelper.FadeInAction(animationLength);
                var normalizeWaitTask = UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f
                                      , cancellationToken:token);
                await UniTask.WhenAll(fadeTask, normalizeWaitTask);
                UnityEngine.Object.Destroy(owner.owerObj);
            }
            catch (OperationCanceledException) { }
        }

        float GetStateAnimationLength()
        {
            var clipName = "Death";
            var clipLength = animator.GetControllerLength(clipName);
            var stateSpeed = animator.GetStateSpeed(clipName);
            return clipLength / stateSpeed;
        }
    }

}

