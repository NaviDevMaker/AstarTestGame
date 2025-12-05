using Cysharp.Threading.Tasks;
using Game.Enemy;
using System.Linq;
using System;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Game.Effect;

namespace Game.Player
{
    public class PlayerSpecialMoveState : PlayerStateMachineBase<PlayerController>,IAnimatorLayer
    {
        public PlayerSpecialMoveState(PlayerController controller) : base(controller) { }
        public int layerIndex { get; private set; }
        public bool isSpecialAttacking { get; private set; }    
        public override void OnEnter() { }
        public override void OnExit() { }
        public override void OnUpdate() { }
        CancellationToken token => controller.GetCancellationTokenOnDestroy();
        public override void Initialize()
        {
            base.Initialize();
            LayerSet();
        }
        public async UniTask SpecialMove()
        {
            if (controller._playerAttackState.isAttacking || isSpecialAttacking) return;
            isSpecialAttacking = true;
            controller.enemyDestroyCount = 0;
            controller.SetHashToFalse();
            controller.OnInvokedSpecialMove();
            controller.animator.SetBool(animatorHash, true);
            controller.audioHelper.PlaySpecialMoveAudio();
            var pos = controller.transform.position + Vector3.up * 0.1f;
            var effect = EffectManager.Instance.specialMoveEffect.GetEffect(pos);
            effect.Play();
            try
            {
                Func<bool> waitAttackAnim = () =>
                {
                    var isDead = controller.isDead;
                    if (isDead) return false;
                    if (controller.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationClipName)) return true;
                    return false;
                };

                await UniTask.WaitUntil(waitAttackAnim, cancellationToken: token);
                await UniTask.WhenAll(UniTask.WaitUntil(() => GetCurrentNormalizeTime(layerIndex) >= 0.95f)
                                     , ExtaminateAroundPlayer()) ;
            }
            catch (OperationCanceledException) { }
            finally
            {
                isSpecialAttacking = false;
                controller.animator.SetBool(animatorHash, false);
                Debug.Log("UŒ‚I‚í‚è‚Å‚·");
            }
        }
        public void LayerSet() => layerIndex = controller.animationData.AttackLayerIndex;

        async UniTask ExtaminateAroundPlayer()
        {
            controller.currentTarget = null;
            var waitTime = 0.2f;
            var copied = controller.specialMoveTargets.ToList();
            try
            {
                foreach (var target in copied)
                {
                    if(target == null) continue;
                    if (target.owerObj == null) continue;
                    EffectManager.Instance.hitEffect.SpawnHitEffect(target.owerObj);
                    target.OnDeadAction?.Invoke(target);
                    controller.OnKillEnemyAction?.Invoke(target);
                    await UniTask.Delay(TimeSpan.FromSeconds(waitTime / 2));
                    await HitStopManager.Instance.HitStop(waitTime / 2);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                controller.specialMoveTargets.Clear();
                controller.specialMoveTargets.TrimExcess();
            }         
        }
    }
}

