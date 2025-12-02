using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
public class ButtonManager : MonoBehaviour
{
    [SerializeField] Button huntButton;
    [SerializeField] Button recordButton;
    public async UniTask Initialize()
    {
        huntButton.transform.localScale = Vector3.zero;
        recordButton.transform.localScale = Vector3.zero;
        huntButton.interactable = false;
        recordButton.interactable = false;

        huntButton.onClick.AddListener(() =>
        {
            SceneTransitonController.Instance.LoadSceneAsync(Scenes.Battle).Forget();
        });
        await ButtonScaleAction();
        huntButton.interactable = true;
        recordButton.interactable = true;
    }
    async UniTask ButtonScaleAction()
    {   
        var huntButtonTask = GetScaleTask(huntButton);
        var recordButtonTask = GetScaleTask(recordButton);
        await huntButtonTask();
        await recordButtonTask();
    }
    Func<UniTask> GetScaleTask(Button targetButton)
    {
        return async () =>
        {
            var token = targetButton.GetCancellationTokenOnDestroy();
            var amount = 1.5f;
            var targetScale = Vector3.one * amount;
            var duration = 1.0f;
            await targetButton.gameObject.Scaler(new Vector3TweenSetup(targetScale, duration / 2))
                                   .ToUniTask(cancellationToken:token);
            await targetButton.gameObject.Scaler(new Vector3TweenSetup(Vector3.one, duration / 2))
                                   .ToUniTask(cancellationToken:token);
        };
    }
}
