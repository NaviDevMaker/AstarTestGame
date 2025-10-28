using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
namespace Game.Player
{
    public class PlayerAttackState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerAttackState(PlayerController controller) : base(controller) { }

        float startNormalizeTime = 0f;
        int layerIndex = 0;
        public bool isAttacking { get; private set; } = false;
        public override void OnEnter() { }
        public override void OnExit() {}

        public override void OnUpdate(){}

        public override void Initialize()
        {
            base.Initialize();
            layerIndex = controller.animationData.AttackLayerIndex;
        }
        public async void Attack()
        {
            if (isAttacking) return;
            try
            {
                isAttacking = true;
                controller.animator.Play(animationClipName);
                Func<bool> waitAttackAnim =  () =>
                {
                    var isDead = controller.isDead;
                    if (isDead) return false;
                    if (controller.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationClipName)) return true;
                    return false;
                };

                await UniTask.WaitUntil(waitAttackAnim, cancellationToken: controller.GetCancellationTokenOnDestroy());
                startNormalizeTime = controller.animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
                while(GetCurrentNormalizeTime() < 0.99f && !controller.isDead)
                {
                    Debug.Log($"{GetCurrentNormalizeTime()},UŒ‚ˆ—’†");
                    await UniTask.Yield(cancellationToken: controller.GetCancellationTokenOnDestroy());
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                isAttacking = false;
            }
        }
        
        float GetCurrentNormalizeTime()
        {
            var now = controller.animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
            return now - startNormalizeTime;
        }
    }
}

