using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Player;
using System.Threading;
using UnityEngine;

public class FogController : MonoBehaviour
{

    [SerializeField] FogFeature fogFeature;
    const float duration = 2.0f;
    CancellationToken token => this.GetCancellationTokenOnDestroy();
    public void Initialize(PlayerController player) => player.OnDeadAction += async (_) => await FogDensityChanger();
    async UniTask FogDensityChanger()
    {
        var mat = fogFeature.runTimeMat;
        var targetDensity = 0f;
        await DOTween.To(
            () => mat.GetFloat("_FogDensity"),
            d => mat.SetFloat("_FogDensity",d),
            targetDensity,
            duration
            ).ToUniTask(cancellationToken:token);
    }
}
