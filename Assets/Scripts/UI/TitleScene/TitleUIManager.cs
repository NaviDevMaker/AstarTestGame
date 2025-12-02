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
        public async UniTask Initialize()
        {
            await TitleTextAction();      
        }
        async UniTask TitleTextAction()
        {
            var token = titleText.GetCancellationTokenOnDestroy();
            var color = titleText.color;
            color.a = 0f;
            titleText.color = color;
            var targetPos = Vector2.zero;
            var duration = 3.0f;
            var moveSet = new Vector2TweenSetup(targetPos,duration);
            var fadeSet = new FadeSet(1.0f,duration);

            await UniTask.WhenAll(titleText.RectMover(moveSet).ToUniTask(cancellationToken: token)
                                  , titleText.Fader(fadeSet).ToUniTask(cancellationToken: token));
        }
    }
}

