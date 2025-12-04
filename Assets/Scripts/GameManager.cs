using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.IO;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{

    public SaveDataController SaveController { get; private set;} = new SaveDataController();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (File.Exists(Path.Combine(Application.persistentDataPath, "saveData.json"))) File.Delete(Path.Combine(Application.persistentDataPath, "saveData.json"));
    //    }
    //}
    public void OnSceneLoaded(Scenes newScene)
    {
        switch (newScene)
        {
            case Scenes.Battle:
                BattleSetUp(); break;
            case Scenes.Title:
                TitleSetUp(); break;
        }
    }
    void BattleSetUp()
    {
        var battleManager = GameObject.FindAnyObjectByType<BattleManager>();
        if (battleManager == null) throw new System.Exception("The bagttle manager isn't founded!!");
        battleManager.Initialize().Forget();
    }
    void TitleSetUp()
    {

    }
}
