using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TextRenewer
{
    public class BattleStartCounter : MonoBehaviour
    {
        [SerializeField] int startCount;
        Text countDownText;
        readonly Vector3 targetScale = Vector3.one * 3f;
        public async UniTask StartCountDown()
        {
            countDownText = GetComponent<Text>();
            if (countDownText == null) return;
            countDownText.text = startCount.ToString();
            countDownText.transform.localScale = Vector3.zero;
            Func<Vector3, UniTask> scaleTask = async (targetScale) =>
            {
                var scaleUpSet = new Vector3TweenSetup(targetScale, 1f);
                await countDownText.gameObject.Scaler(scaleUpSet);
                countDownText.transform.localScale = Vector3.zero;
            };
            for (int i = startCount - 1; i >= 0; i--)
            {
                AudioManager.Instance.PlayCountDownSE(i == 0);
                await scaleTask(targetScale);
                countDownText.text = (--startCount).ToString();
                if (i == 0)
                {
                    var finalTargetScale = Vector3.one * 130f;
                    scaleTask(finalTargetScale).Forget();
                }
            }
        }
    }
}

