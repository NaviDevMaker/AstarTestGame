using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
public class RecordManager : MonoBehaviour
{
    [SerializeField] Text recordText;
    [SerializeField] Text timeText;

    const string nonRecordContent = "NO RECORD...";
    private void Start()
    {
        var c1 = recordText.color;
        var c2 = timeText.color;
        c1.a = 0f;
        c2.a = 0f;
        recordText.color = c1;
        timeText.color = c2;
    }
    public async UniTask ShowAndWaitClose()
    {
        var data = GameManager.Instance.SaveController.LoadData();
        var score = data.highScore;
        var time = data.highestDateTime;
        var targetTexts = new List<Text>();
        if (score == -1)
        {
            recordText.text = nonRecordContent;
            targetTexts.Add(recordText);
        }
        else
        {
            recordText.text = $"High Score:{score.ToString("D3")}" ;
            timeText.text = $"Date Achieved:{time.ToString()}" ;
            targetTexts = new List<Text>() { recordText, timeText };
        }

        try
        {
            await UniTask.WhenAll(UIActionHelper.GetUIFadeTask(targetAlpha:1.0f,targetTexts.Cast<Graphic>().ToArray()));
            await UniTask.WaitUntil(() => InputManager.RecordUICloseButtonPressed()
                                   , cancellationToken: this.GetCancellationTokenOnDestroy());
            await UniTask.WhenAll(UIActionHelper.GetUIFadeTask(targetAlpha: 0f, targetTexts.Cast<Graphic>().ToArray()));
        }
        catch (OperationCanceledException){ }
    }
}
