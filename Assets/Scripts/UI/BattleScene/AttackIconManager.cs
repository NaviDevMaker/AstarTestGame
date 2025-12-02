using Game.Player;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
namespace Game.Icon
{
    public class AttackIconManager : MonoBehaviour
    {
        [SerializeField] Image attackIconImage;
        //PlayerController player;
        //Func<bool> waitAttackingEnd;
        Color originalColor;
        Color newColor;
        Vector3 originalScale;
        CancellationToken token => this.GetCancellationTokenOnDestroy();
        CancellationTokenSource scaleCts = null;
        public void Initialize(PlayerController player)
        {
            //this.player = player;
            //waitAttackingEnd = () => player._playerAttackState.isAttacking;
            player.OnAttackingAction += WaitAttackingEnd;
            originalColor = attackIconImage.color;
            if (!ColorUtility.TryParseHtmlString("#FF0000", out var newColor)) 
                throw new Exception("The required color isn't exist!!");
            this.newColor = newColor;
            originalScale = attackIconImage.transform.localScale;
        }
        async UniTask WaitAttackingEnd(float animLength)
        {
            attackIconImage.fillAmount = 0f;
            attackIconImage.color = newColor;
            var targetAmount = 1.0f;
            var fillTween = DOTween.To(                
                    () => attackIconImage.fillAmount,
                    f => attackIconImage.fillAmount = f,
                    targetAmount,
                    animLength
                ).SetUpdate(UpdateType.Normal,true)
                .SetEase(Ease.Linear);
            try
            {
                await fillTween.ToUniTask(cancellationToken: token);
                await GetScaleTask();
            }
            catch (OperationCanceledException) { }
            finally {attackIconImage.color = originalColor;}
        }
        async UniTask GetScaleTask()
        {
            scaleCts?.Cancel();
            scaleCts?.Dispose();
            scaleCts = new CancellationTokenSource();

            var doubleToken = CancellationTokenSource.CreateLinkedTokenSource(token, scaleCts.Token);
            var amount = 2.0f;
            var duration = 0.1f;
            var ease = Ease.Linear;
            var scaleSet = new Vector3TweenSetup(originalScale * amount,duration / 2, ease);
            var scaleToOriginal = new Vector3TweenSetup(originalScale, duration / 2, ease);
            var seq = DOTween.Sequence();
            seq.Append(attackIconImage.gameObject.Scaler(scaleSet)
                     .SetUpdate(UpdateType.Normal, true))
                .Append(attackIconImage.gameObject.Scaler(scaleToOriginal)
                     .SetUpdate(UpdateType.Normal, true));
                
            await seq.ToUniTask(tweenCancelBehaviour:TweenCancelBehaviour.Complete
                               ,cancellationToken:doubleToken.Token);

        }
    }
}
