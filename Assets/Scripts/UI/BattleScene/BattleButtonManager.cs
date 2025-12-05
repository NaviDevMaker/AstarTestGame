using Cysharp.Threading.Tasks;
using Game.Player;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleButtonManager : MonoBehaviour
{
    [SerializeField] Button titleButton;
    [SerializeField] Button retryButton;
    public void Initialize()
    {
        ButtonSetup();
        var player = GameObject.FindAnyObjectByType<PlayerController>();
        player.OnDeadAction += async (_) => { await ButttonAction();};
    }
    void ButtonSetup()
    {
        titleButton.transform.localScale = Vector3.zero;
        retryButton.transform.localScale = Vector3.zero;
        titleButton.interactable = false;
        retryButton.interactable = false;

        var sceneManager = SceneTransitonController.Instance;
        titleButton.onClick.AddListener(() => sceneManager.LoadSceneAsync(Scenes.Title).Forget());
        retryButton.onClick.AddListener(() => sceneManager.LoadSceneAsync(Scenes.Battle).Forget());
    }
    public async UniTask ButttonAction()
    {
        try
        {
            var waitTime = 3.0f;
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime),cancellationToken: this.GetCancellationTokenOnDestroy());
            var duration = 0.5f;
            var amount = 2.0f;
            await UIActionHelper.UIScaleAction(duration, amount
                                               ,graphics: new Graphic[] {titleButton.image, retryButton.image });
            titleButton.interactable = true;
            retryButton.interactable = true;
        }
        catch (OperationCanceledException) { }
    }
}
