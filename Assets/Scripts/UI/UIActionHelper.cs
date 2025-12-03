using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
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
