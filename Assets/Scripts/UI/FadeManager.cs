using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class FadeManager : SingletonMonobehaviour<FadeManager>
{
    [SerializeField] float duration;
    [SerializeField] Image fadeImage;
    CancellationToken token => this.GetCancellationTokenOnDestroy();
    public async UniTask FadeAction(float targetAlpha)
    {
        var startAlpha = fadeImage.color.a;
        Debug.Log($"Start Alpha,{startAlpha}");
        var elapsedTime = 0f;

        try
        {
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var lerp = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                var color = fadeImage.color;
                color.a = lerp;
                fadeImage.color = color;
                await UniTask.Yield(cancellationToken: token);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            var finalColor = fadeImage.color;
            finalColor.a = targetAlpha;
            fadeImage.color = finalColor;
        }
    }
    public async UniTask FadeIn() => await FadeAction(targetAlpha:1.0f);
    public async UniTask FadeOut() => await FadeAction(targetAlpha:0f);
}
