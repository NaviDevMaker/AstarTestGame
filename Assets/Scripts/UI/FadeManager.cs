using Cysharp.Threading.Tasks;
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
        var startAlpha = targetAlpha == 1.0f
                         ? 0f
                         : 1.0f;
        Debug.Log($"Start Alpha,{startAlpha}");
        var elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var lerp = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            var color = fadeImage.color;
            color.a = lerp;
            fadeImage.color = color;
            await UniTask.Yield(cancellationToken:token);
        }
    }
    public async UniTask FadeIn() => await FadeAction(targetAlpha:1.0f);
    public async UniTask FadeOut() => await FadeAction(targetAlpha:0f);
}
