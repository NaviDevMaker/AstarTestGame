using Cysharp.Threading.Tasks;
using Game.TitleUI;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] TitleUIManager titleUIManager;
    [SerializeField] TitleButtonManager buttonManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize().Forget();
    }
    async UniTask Initialize()
    {
       await titleUIManager.Initialize();
       await buttonManager.Initialize();
    }
}
