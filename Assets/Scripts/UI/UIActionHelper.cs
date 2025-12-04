using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public static class UIActionHelper
{
    public static async UniTask UIScaleAction(float duration, float amount,params Graphic[] graphics)
    {
        for (int i = 0; i < graphics.Length; i++)
        {
            var graphic = graphics[i];
            var buttonTask = GetScaleTask(graphic,duration,amount);
            await buttonTask();
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
    public static Func<UniTask> GetScaleTask(Graphic targetGraphic,float duration,float amount)
    {
        return async () =>
        {
            var token = targetGraphic.GetCancellationTokenOnDestroy();
            var targetScale = Vector3.one * amount;
            await targetGraphic.gameObject.Scaler(new Vector3TweenSetup(targetScale, duration / 2))
                                   .ToUniTask(cancellationToken: token);
            await targetGraphic.gameObject.Scaler(new Vector3TweenSetup(Vector3.one, duration / 2))
                                   .ToUniTask(cancellationToken: token);
        };
    }
}
