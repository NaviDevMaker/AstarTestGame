using Cysharp.Threading.Tasks;
using Game.TitleUI;
using System;
using UnityEngine;
using UnityEngine.UI;
public class TitleButtonManager : MonoBehaviour
{
    [SerializeField] Button huntButton;
    [SerializeField] Button recordButton;
    [SerializeField] RecordManager recordManager;
    [SerializeField] TitleUIManager titleUIManager;
    public async UniTask Initialize()
    {
        huntButton.transform.localScale = Vector3.zero;
        recordButton.transform.localScale = Vector3.zero;
        huntButton.interactable = false;
        recordButton.interactable = false;

        huntButton.onClick.AddListener(() =>
        {
            SceneTransitonController.Instance.LoadSceneAsync(Scenes.Battle).Forget();
            AudioManager.Instance.PlayBattleSE();
        });

        recordButton.onClick.AddListener(() =>
        {
            OnClickedRecordButton().Forget();
            AudioManager.Instance.PlayRecordSE();
        });
        var amount = 1.5f;
        var duration = 1.0f;
        await UIActionHelper.UIScaleAction(duration,amount,graphics:new Graphic[] {huntButton.image,recordButton.image });
        huntButton.interactable = true;
        recordButton.interactable = true;
    }
    async UniTask OnClickedRecordButton()
    {
        Debug.Log("レコードボタンが押されました");
        huntButton.interactable = false;
        recordButton.interactable = false;
        var graphics = new Graphic[] {huntButton.image,recordButton.image
                                    ,huntButton.GetComponentInChildren<Text>(),recordButton.GetComponentInChildren<Text>()};

        Func<float, UniTask> fadeTask = async(alpha)
                                      => await UIActionHelper.GetUIFadeTask(targetAlpha: alpha, graphics);
        Func<float,UniTask> titleTextTask = async(targetAlpha) => await titleUIManager.TitleTextAction(targetAlpha: targetAlpha);
        await UniTask.WhenAll(fadeTask(0f),recordManager.ShowAndWaitClose()
            　　　　　　　　　,titleTextTask(0f));
        await UniTask.WhenAll(fadeTask(1f),titleTextTask(1.0f)) ;
        huntButton.interactable = true;
        recordButton.interactable = true;
    }
}
