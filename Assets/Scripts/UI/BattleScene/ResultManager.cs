using Cysharp.Threading.Tasks;
using Game.Player;
using Game.TextRenewer;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class ResultManager : MonoBehaviour
{
    [SerializeField] Text resultText;
    [SerializeField] Text newRecordText;
    [SerializeField] ScoreManager scoreManager; 
    public void Initialize(PlayerController player)
    {
        resultText.transform.localScale = Vector3.zero;
        newRecordText.transform.localScale = Vector3.zero;
        player.OnDeadAction += async (_) => await ResultAppear();
    }
    async UniTask ResultAppear()
    {
        var result = scoreManager.currentScore;
        resultText.text = result.ToString("D3");
        var duration = 2.0f;
        var amount = 2.0f;
        await UIActionHelper.UIScaleAction(duration,amount,resultText);
        var gameManager = GameManager.Instance;
        var previousHighScore = gameManager.SaveController.LoadData().highScore;
        gameManager.SaveController.SaveData(result);
        if (previousHighScore >= result) return;
        await UIActionHelper.UIScaleAction(duration,amount,newRecordText);
        StartLoopAction().Forget();
    }
    async UniTask StartLoopAction()
    {
        try
        {
            CancellationToken token = newRecordText.GetCancellationTokenOnDestroy();
            var baseAmount = newRecordText.transform.localScale.x;
            var scaleSpeed = 60.0f;
            var elapsedTime = 0f;
            while(true)
            {
                token.ThrowIfCancellationRequested();
                var amount = Mathf.Sin(scaleSpeed * elapsedTime * Mathf.Deg2Rad) + baseAmount;
                var targetScale = Vector3.one *(baseAmount + amount);
                newRecordText.transform.localScale = targetScale;
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }
        }
        catch (OperationCanceledException) { }
    }
}
