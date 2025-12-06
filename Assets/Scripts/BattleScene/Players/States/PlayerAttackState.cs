using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Game.Enemy;
using Game.Effect;

public interface IAnimatorLayer
{ 
    int layerIndex { get; }
    void LayerSet();
}

namespace Game.Player
{
    public class PlayerAttackState : PlayerStateMachineBase<PlayerController>,IAnimatorLayer
    {
        public PlayerAttackState(PlayerController controller) : base(controller) { }

        public int layerIndex { get; private set;}
        float attackbleNorTime = 0f;
        public bool isAttacking { get; private set; } = false;
        public override void OnEnter() { }
        public override void OnExit() {}

        public override void OnUpdate(){}

        public async override void Initialize()
        {
            base.Initialize();
            LayerSet();
            attackbleNorTime = await GetAttackableNormalizeTime();
        }
        public async UniTask Attack()
        {
            if (isAttacking || controller._playerSpecialMoveState.isSpecialAttacking) return;
            var token = controller.GetCancellationTokenOnDestroy();
            try
            {
                isAttacking = true;
                controller.OnAttackingAction(animLength).Forget();
                controller.SetHashToFalse();
                controller.animator.Play(animationClipName, layerIndex);
                controller.animator.SetBool(animatorHash, true);
                controller.audioHelper.PlayAttackAudio();
                Func<bool> waitAttackAnim =  () =>
                {
                    var isDead = controller.isDead;
                    if (isDead) return false;
                    if (controller.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationClipName)) return true;
                    return false;
                };

                await UniTask.WaitUntil(waitAttackAnim, cancellationToken: token);
                while(GetCurrentNormalizeTime(layerIndex) < attackbleNorTime && !controller.isDead)
                {
                    Debug.Log($"{GetCurrentNormalizeTime(layerIndex)},{controller.currentTarget},çUåÇèàóùíÜ");
                    if(controller.currentTarget != null)
                    {
                        Debug.Log("taosu");
                        var currentTarget = controller.currentTarget;
                        EffectManager.Instance.hitEffect.SpawnHitEffect(currentTarget.owerObj);
                        currentTarget.OnDeadAction?.Invoke(currentTarget);
                        controller.enemyDestroyCount ++;//controller.enemyDestroyCount++;
                        controller.OnKillEnemyAction?.Invoke(currentTarget);
                        HitStopManager.Instance.HitStop(0.5f).Forget();
                        controller.currentTarget = null;
                        break;
                    }
                    await UniTask.Yield(cancellationToken: token);
                }

                await UniTask.WaitUntil(() => GetCurrentNormalizeTime(layerIndex) >= 0.95f);
            }
            catch (OperationCanceledException){}
            finally
            {
                isAttacking = false;
                controller.animator.SetBool(animatorHash, false);
                Debug.Log("çUåÇèIÇÌÇËÇ≈Ç∑");
            }
           
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
        public void LayerSet() => layerIndex = controller.animationData.AttackLayerIndex;
    }
}

