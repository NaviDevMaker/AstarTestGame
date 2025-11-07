using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
namespace Game.Player
{
    public class PlayerHitState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerHitState(PlayerController controller) : base(controller) { }
        float duration = 0f;
        public override void OnEnter() { }
        public override void OnExit() { }
        public override void OnUpdate() { }
        public async void WaitInvincibleTime()
        {
            if (controller.isDead) return;
            controller.isInvincible = true;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: controller.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException) { }
            finally { controller.isInvincible = false; }
        }
        public override void Initialize() => duration = controller.playerStatusData.InvincibleDuration;
    }
}


