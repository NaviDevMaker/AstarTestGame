using Cysharp.Threading.Tasks;
using Game.Player;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Icon
{
    public class SpecialMoveIconManager:MonoBehaviour
    {
        [SerializeField] Image specialMoveIcon;
        [SerializeField] Text specialMoveText;

        Color originalIconColor;
        Color originalTextColor;
        Color newColor;

        Func<float> GetTargetFillRatio;
        CancellationToken token => this.GetCancellationTokenOnDestroy();
        CancellationTokenSource fillCts;
        public void Initialize(PlayerController player)
        {
            originalIconColor = specialMoveIcon.color;
            originalTextColor = specialMoveText.color;
            if (!ColorUtility.TryParseHtmlString("#FF0000", out var newColor)) throw new System.Exception("Not found color...");
            this.newColor = newColor;
            GetTargetFillRatio += () =>
            {
                var max = (float)player.playerStatusData.SpecialMovableCount;
                var current = (float)player.enemyDestroyCount;
                return current / max;
            };

            ResetStatus();
            player.OnInvokedSpecialMove += ResetStatus;
            player.OnKillEnemyAction += async (_) => await FillAction();
        }
        async UniTask UIActionTask()
        {
            var amount = 2.0f;
            var duration = 0.5f;
            await UIActionHelper.UIScaleAction(duration, amount, graphics: new Graphic[]{specialMoveIcon,specialMoveText});
            specialMoveIcon.color = originalIconColor;
            specialMoveText.color = originalTextColor;
        }
        void ResetStatus()
        {
            specialMoveIcon.fillAmount = 0f;
            specialMoveIcon.color = newColor;
            specialMoveText.color= newColor;
        }
        async UniTask FillAction()
        {
            if (specialMoveIcon.fillAmount == 1.0f) return;
            fillCts?.Cancel();
            fillCts?.Dispose();
            fillCts = new CancellationTokenSource();
            var doubleCts = CancellationTokenSource.CreateLinkedTokenSource(token, fillCts.Token);
            var duration = 0.2f;
            try
            {
                await specialMoveIcon.GetFillTween(GetTargetFillRatio(), duration)
                    .ToUniTask(tweenCancelBehaviour: TweenCancelBehaviour.Complete, cancellationToken:doubleCts.Token);
            }
            catch (OperationCanceledException) { }
            
            if (specialMoveIcon.fillAmount == 1.0f) UIActionTask().Forget();      
        }
    }
}

