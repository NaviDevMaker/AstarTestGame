using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
public class SceneTransitonController : SingletonMonobehaviour<SceneTransitonController>
{

    [SerializeField] List<SceneData> sceneDatas;
    [System.Serializable]
    class SceneData
    {
        [SerializeField] Scenes scene;
        [SerializeField] string name;
        public Scenes Scene => scene;
        public string Name => name;
    }
    public async UniTask LoadSceneAsync(Scenes nextScene)
    {
        await FadeManager.Instance.FadeIn();
        var nextSceneName = GetNextScene(nextScene);
        Debug.Log($"ŽŸ‚ÌƒV[ƒ“‚Ì–¼‘O,{nextSceneName}");
        await SceneManager.LoadSceneAsync(nextSceneName);
        GameManager.Instance.OnSceneLoaded(nextScene);
        await FadeManager.Instance.FadeOut();
    }
    string GetNextScene(Scenes nextScene)
    {
        return sceneDatas.FirstOrDefault(d => d.Scene == nextScene)
               ?.Name;
    }
}
public enum Scenes
{ 
    Battle,
    Title,
}

