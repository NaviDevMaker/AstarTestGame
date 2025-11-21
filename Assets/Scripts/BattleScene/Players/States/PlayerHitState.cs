using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using System.Net.WebSockets;
using System.Threading;
namespace Game.Player
{
    public class PlayerHitState : PlayerStateMachineBase<PlayerController>
    {
        public PlayerHitState(PlayerController controller) : base(controller) { }
        class TweenInfo
        {
             public readonly float duration = 0f;
             public readonly float blinkDuration = 0f;
             public readonly float targetAlpha = 0f;

            public TweenInfo(float duration,float blinkDuration,float targetAlpha)
            {
                this.duration = duration;
                this.blinkDuration = blinkDuration;
                this.targetAlpha = targetAlpha;
            }
        }

        CancellationToken token;
        TweenInfo tweenInfo;
        List<Material> meshMats = new List<Material>();
        public override void OnEnter() { }
        public override void OnExit() { }
        public override void OnUpdate() { }
        public async void WaitInvincibleTime()
        {
            if (controller.isDead) return;
            controller.isInvincible = true;
            try
            {
                await LitMaterials();
            }
            catch (OperationCanceledException) { }
            finally { controller.isInvincible = false; }
        }
        public override void Initialize()
        {
            SetMaterials();
            SetTweenInfo();
            token = controller.GetCancellationTokenOnDestroy();
        }

        void SetTweenInfo()
        {
           var duration = controller.playerStatusData.InvincibleDuration;
           var blinkDuration = controller.playerTweenFieldDatas.blinkDuration;
           var targetAlpha = controller.playerTweenFieldDatas.TargetAlpha;
            tweenInfo = new TweenInfo(duration, blinkDuration, targetAlpha);
        }
        void SetMaterials()
        {
            var mats = controller.meshMats;
            meshMats = mats.SelectMany(mats => mats.Select(m => m))
                       .ToList();
        }
        async UniTask LitMaterials()
        {
            var tasks = meshMats.Select(m =>
            {
                var originalColor = m.color;
                return GetBlinkSequence(m).ToUniTask(cancellationToken:token);
            }).ToList();

            await tasks;
        }

        Sequence GetBlinkSequence(Material material)
        {
            var duration = tweenInfo.duration;
            var blinkDuration = tweenInfo.blinkDuration;    
            var loopCount = Mathf.RoundToInt(duration / blinkDuration);
            var originalColor = material.color;
            var targetAlpha = tweenInfo.targetAlpha;
            var targetColor = originalColor;
            targetColor.a = targetAlpha;
            var seq = DOTween.Sequence();
            return seq.Append(material.DOColor(targetColor, blinkDuration))
               .Append(material.DOColor(targetColor,blinkDuration))
               .SetLoops(loopCount,LoopType.Yoyo);
        }
    }
}


