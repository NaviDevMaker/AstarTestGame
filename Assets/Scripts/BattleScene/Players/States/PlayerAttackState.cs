using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
namespace Game.Player
{
    public class PlayerAttackState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerAttackState(PlayerController controller) : base(controller) { }

        int layerIndex = 0;
        float attackbleNorTime = 0f;
        public bool isAttacking { get; private set; } = false;
        public override void OnEnter() { }
        public override void OnExit() {}

        public override void OnUpdate(){}

        public async override void Initialize()
        {
            base.Initialize();
            layerIndex = controller.animationData.AttackLayerIndex;
            attackbleNorTime = await GetAttackableNormalizeTime();
        }
        public async void Attack()
        {
            if (isAttacking) return;
            var token = controller.GetCancellationTokenOnDestroy();
            try
            {
                isAttacking = true;
                controller.animator.Play(animationClipName);
                controller.animator.SetBool(animatorHash, true);
                Func<bool> waitAttackAnim =  () =>
                {
                    var isDead = controller.isDead;
                    if (isDead) return false;
                    if (controller.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationClipName)) return true;
                    return false;
                };

                await UniTask.WaitUntil(waitAttackAnim, cancellationToken: token);
                while(GetCurrentNormalizeTime() < attackbleNorTime && !controller.isDead)
                {
                    Debug.Log($"{GetCurrentNormalizeTime()},{controller.currentTarget},çUåÇèàóùíÜ");
                    if(controller.currentTarget != null)
                    {
                        Debug.Log("taosu");
                        controller.currentTarget.OnDeadAction?.Invoke();
                        //controller.currentTarget = null;
                        break;
                    }
                    await UniTask.Yield(cancellationToken: token);
                }

                await UniTask.WaitUntil(() => GetCurrentNormalizeTime() >= 0.99f);
            }
            catch (OperationCanceledException){ controller.animator.SetBool(animatorHash,false); }
            isAttacking = false;
            controller.animator.SetBool(animatorHash, false);
            Debug.Log("çUåÇèIÇÌÇËÇ≈Ç∑");
        }       
        float GetCurrentNormalizeTime()
        {
            var now = controller.animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
            return now % 1;
        }
        async UniTask<float> GetAttackableNormalizeTime()
        {
            var clip = await controller.animationData.LoadClip(animationClipName);
            var length = clip.length;
            var frameRate = clip.frameRate;
            var maxFrame = length * frameRate;
            var attackEndFrame = controller.playerStatusData.AttackEndFrame;
            return attackEndFrame / maxFrame;
        }
    }
}

