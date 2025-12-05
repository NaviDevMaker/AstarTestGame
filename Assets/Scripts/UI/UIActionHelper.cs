using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
public static class UIActionHelper
{
    public static async UniTask UIScaleAction(float duration, float firstAmount
                                             ,float finalAmount = 1.0f,params Graphic[] graphics)
    {
        for (int i = 0; i < graphics.Length; i++)
        {
            var graphic = graphics[i];
            var graphicTask = GetScaleTask(graphic,duration,firstAmount,finalAmount);
            await graphicTask();
        }
    }
    public static UniTask[] GetUIFadeTask(float targetAlpha, params Graphic[] targetTexts)
    {
        var fadeDuration = 2.0f;
        return targetTexts.Select(text =>
        {
            var fadeSet = new FadeSet(targetAlpha, fadeDuration);
            return text.Fader(fadeSet).ToUniTask(cancellationToken: text.GetCancellationTokenOnDestroy());
        }).ToArray();
    }
    public static Func<UniTask> GetScaleTask(Graphic targetGraphic,float duration,float firstAmmount
                                            ,float finalAmount = 1.0f)
    {
        return async () =>
        {
            var token = targetGraphic.GetCancellationTokenOnDestroy();
            var targetScale = Vector3.one * firstAmmount;
            await targetGraphic.gameObject.Scaler(new Vector3TweenSetup(targetScale, duration / 2))
                                   .ToUniTask(cancellationToken: token);
            await targetGraphic.gameObject.Scaler(new Vector3TweenSetup(Vector3.one * finalAmount, duration / 2))
                                   .ToUniTask(cancellationToken: token);
        };
    }

    public static Tween GetFillTween(this Image targetImage,float targetAmount,float length)
    {
        return  DOTween.To(
                    () => targetImage.fillAmount,
                    f => targetImage.fillAmount = f,
                    targetAmount,
                    length
                ).SetUpdate(UpdateType.Normal, true)
                .SetEase(Ease.Linear);
    }
    
}
