using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;

namespace Game.Player
{
    public class FPSCameraController : MonoBehaviour
    {
        [Header("Duration")]
        [SerializeField] float duration;

        [Header("Shake Info")]
        [SerializeField] float strength;
        [SerializeField] int vibrato;
        [SerializeField] float randomness;
        PlayerController player;
        Vector3 originalPos;
        CancellationTokenSource shakeCts = null;
        private void Awake() => Initialize();
        public async void ShakeCamera()
        {
            shakeCts?.Cancel();
            shakeCts?.Dispose();
            shakeCts = new CancellationTokenSource();
            try
            {
                var tween = transform.DOShakePosition(duration, strength, vibrato, randomness);
                var shakeTask = tween.ToUniTask(cancellationToken: shakeCts.Token);
                await shakeTask;
            }
            catch (OperationCanceledException) { }
            finally { transform.localPosition = originalPos; }
        }
        void Initialize()
        {
            originalPos = transform.localPosition;
            player = transform.parent.GetComponent<PlayerController>();
            player.OnHitEnemyAction += ShakeCamera;
            player.OnDeadAction += RotateCamera;
        }
        async UniTask RotateCamera(float rotateDuration)
        {
            //return async() =>
            //{
                var targetRot = new Vector3(-90f, 0f, 0f);
                var rotateSet = new Vector3TweenSetup(targetRot, rotateDuration);
                var rotateTask = transform.gameObject.Roter(rotateSet)
                                 .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
                try { await rotateTask; }
                catch (OperationCanceledException) { throw; }
            //};        
        }
    }
}

