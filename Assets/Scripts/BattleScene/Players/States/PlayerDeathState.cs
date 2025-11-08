using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
namespace Game.Player
{
    public class PlayerDeathState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerDeathState(PlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.animator.SetTrigger(animatorHash);
            DownAction().Forget();
        }
        public override void OnExit() { }
        public override void OnUpdate() { }
        async UniTask DownAction()
        {
            var layerIndex = controller.animationData.BaseLayerIndex;
            var amount = 1.2f;
            var downOffset = Vector3.down * amount;
            var token = controller.GetCancellationTokenOnDestroy();
            var animator = controller.animator;
            try
            {
                await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationClipName)
                                         , cancellationToken: token);
                var stateSpeed = animator.GetCurrentAnimatorStateInfo(0).speed;
                var duration = this.length * stateSpeed;
                var targetPos = controller.transform.position + downOffset;
                var ease = Ease.Linear;
                var moveSet = new Vector3TweenSetup(targetPos, duration, ease);
                var moveTask = controller.gameObject.Mover(moveSet).ToUniTask(cancellationToken: token);
                var fpsCameraTask = controller.OnDeadAction(duration);
                await UniTask.WhenAll(moveTask,fpsCameraTask);
            }
            catch (OperationCanceledException) { }
        }
    }
}

