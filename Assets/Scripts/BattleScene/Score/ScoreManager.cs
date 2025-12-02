using UnityEngine;
using UnityEngine.UI;
using Game.Enemy;
using Game.Player;
using DG.Tweening;

namespace Game.TextRenewer
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] TweenAmountSetting tweenAmountSetting;
        [SerializeField] UnityEngine.UI.Text scoreText;
        [SerializeField] PlayerController player;
        int currentScore = 0;
        Vector3 originalScale;
        Sequence currentScaleTween = null;

        [System.Serializable]
        class TweenAmountSetting
        {
            [SerializeField] float duration;

            [Header("Scaler")]
            [SerializeField] float scaleAmount;

            [Header("Shaker")]
            [SerializeField] int strength;
            [SerializeField] int vibrato;
            [SerializeField] int radomness;
            public float ScaleAmount => scaleAmount;
            public int Strength => strength;
            public int Vibrato => vibrato;
            public int Randomness => radomness;
            public float Duration => duration;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Initialize()
        {
            scoreText.text = currentScore.ToString();
            player.AddScoreAction += AddScore;
            originalScale = scoreText.transform.localScale;
        }
        void AddScore(IEnemy targetEnemy)
        {
            var score = targetEnemy._enemyStatusData.Score;
            currentScore += score;
            scoreText.text = currentScore.ToString();
            ScaleText();
        }
        void ScaleText()
        {
            if (currentScaleTween != null) currentScaleTween.Kill(complete: true);
            currentScaleTween = DOTween.Sequence()
                                .SetUpdate(UpdateType.Normal, true);
            var amount = tweenAmountSetting.ScaleAmount;
            var duraiton = tweenAmountSetting.Duration;
            var scaleDuration = duraiton / 2;
            var scaleSeq = DOTween.Sequence();
            var scaleSet = new Vector3TweenSetup(new Vector3(originalScale.x, originalScale.y * amount, originalScale.z)
                                                , scaleDuration);
            scaleSeq.Append(scoreText.gameObject.Scaler(scaleSet))
                    .Append(scoreText.gameObject.Scaler(new Vector3TweenSetup(originalScale, scaleDuration)));
            var strength = tweenAmountSetting.Strength;
            var vibrato = tweenAmountSetting.Vibrato;
            var randomness = tweenAmountSetting.Randomness;
            var shakeTween = scoreText.gameObject.Shaker(new ShakeSet(duraiton, strength, vibrato, randomness));

            currentScaleTween.Append(scaleSeq)
                             .Join(shakeTween)
                             .OnComplete(() => currentScaleTween = null);
            currentScaleTween.Play();
        }
    }
}


