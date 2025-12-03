using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
public class TitleButtonManager : MonoBehaviour
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
        var amount = 1.5f;
        var duration = 1.0f;
        await UIActionHelper.UIScaleAction(duration,amount,new Graphic[] {huntButton.image,recordButton.image });
        huntButton.interactable = true;
        recordButton.interactable = true;
    }
    async UniTask OnClickedRecordButton()
    {
        huntButton.interactable = false;
        recordButton.interactable = false;
    }
}
