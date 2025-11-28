using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class HitStopManager : SigletonMonobehaiver<HitStopManager>
{
    CancellationToken destroyedToken;
    CancellationTokenSource hitStopCts;
    protected override void Awake()
    {
        base.Awake();
        destroyedToken = this.GetCancellationTokenOnDestroy();
    }
    public async UniTask HitStop(float stopDuration)
    {
        hitStopCts?.Cancel();
        hitStopCts?.Dispose();
        hitStopCts = new CancellationTokenSource();
        var doubleCts = CancellationTokenSource.CreateLinkedTokenSource(hitStopCts.Token,destroyedToken);
        Time.timeScale = 0f;
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(stopDuration)
                               ,ignoreTimeScale:true
                               , cancellationToken: doubleCts.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            Time.timeScale = 1.0f;
        }
    }
}
