using Game.Player;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System.Linq;
namespace Game.Icon
{
    public class AttackIconManager : MonoBehaviour
    {
        [SerializeField] Image attackIconImage;
        [SerializeField] Text attackText;
        //PlayerController player;
        //Func<bool> waitAttackingEnd;
        Color originalIconColor;
        Color originalTextColor;
        Color newColor;

        Vector3 originalImageScale;
        Vector3 originalTextScale;
        CancellationToken token => this.GetCancellationTokenOnDestroy();
        CancellationTokenSource scaleCts = null;
        public void Initialize(PlayerController player)
        {
            //this.player = player;
            //waitAttackingEnd = () => player._playerAttackState.isAttacking;
            player.OnAttackingAction += WaitAttackingEnd;
            originalIconColor = attackIconImage.color;
            originalTextColor = attackText.color;
            if (!ColorUtility.TryParseHtmlString("#FF0000", out var newColor)) 
                throw new Exception("The required color isn't exist!!");
            this.newColor = newColor;
            originalImageScale = attackIconImage.transform.localScale;
            originalTextScale = attackText.transform.localScale;
        }
        async UniTask WaitAttackingEnd(float animLength)
        {
            attackIconImage.fillAmount = 0f;
            attackIconImage.color = newColor;
            attackText.color = newColor;
            var targetAmount = 1.0f;
            //var fillTween = DOTween.To(                
            //        () => attackIconImage.fillAmount,
            //        f => attackIconImage.fillAmount = f,
            //        targetAmount,
            //        animLength
            //    ).SetUpdate(UpdateType.Normal,true)
            //    .SetEase(Ease.Linear);
            try
            {
                await attackIconImage.GetFillTween(targetAmount,animLength).ToUniTask(cancellationToken: token);
                await UniTask.WhenAll(GetScaleTask(new (Graphic,Vector3)[]{(attackIconImage,originalImageScale),
                                                                            (attackText,originalTextScale)}));
            }
            catch (OperationCanceledException) { }
            finally 
            {
                attackIconImage.color = originalIconColor;
                attackText.color = originalTextColor;
            }
        }
        async UniTask GetScaleTask((Graphic graphic, Vector3 originalScale)[] values)
        {
            scaleCts?.Cancel();
            scaleCts?.Dispose();
            scaleCts = new CancellationTokenSource();

            await values.Select(value =>
            {
                var graphic = value.graphic;
                var originalScale = value.originalScale;
                var doubleToken = CancellationTokenSource.CreateLinkedTokenSource(token, scaleCts.Token);
                var amount = 2.0f;
                var duration = 0.1f;
                var ease = Ease.Linear;
                var scaleSet = new Vector3TweenSetup(originalScale * amount, duration / 2, ease);
                var scaleToOriginal = new Vector3TweenSetup(originalScale, duration / 2, ease);
                var seq = DOTween.Sequence();
                seq.Append(graphic.gameObject.Scaler(scaleSet)
                         .SetUpdate(UpdateType.Normal, true))
                    .Append(graphic.gameObject.Scaler(scaleToOriginal)
                         .SetUpdate(UpdateType.Normal, true));

                return seq.ToUniTask(tweenCancelBehaviour: TweenCancelBehaviour.Complete
                                   , cancellationToken: doubleToken.Token);
            }).ToArray();
        }
    }
}
