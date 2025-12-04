using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TitleUI
{
    public class TitleUIManager : MonoBehaviour
    {
        [SerializeField] Text titleText;
        Vector2 originalPos;
        public async UniTask Initialize()
        {
            originalPos = titleText.rectTransform.localPosition;
            await TitleTextAction(targetAlpha:1.0f);      
        }
        public async UniTask TitleTextAction(float targetAlpha)
        {
            var token = titleText.GetCancellationTokenOnDestroy();
            var color = titleText.color;
            color.a = targetAlpha == 1.0f ? 0f
                                          : 1.0f;
            titleText.color = color;
            var targetPos = targetAlpha == 1.0f ? Vector2.zero
                                                :originalPos;
            var duration = 3.0f;
            var moveSet = new Vector2TweenSetup(targetPos,duration);
            var fadeSet = new FadeSet(targetAlpha, duration);

            await UniTask.WhenAll(titleText.RectMover(moveSet).ToUniTask(cancellationToken: token)
                                  , titleText.Fader(fadeSet).ToUniTask(cancellationToken: token));
        }
    }
}

