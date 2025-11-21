using UnityEngine;
using Game.Player;
using Game.Stage;
using Game.Spawner;
using Cysharp.Threading.Tasks;
public class BattleManager : MonoBehaviour
{
    [SerializeField] PosaitionSetUper positionSetuper;
    [SerializeField] CameraMover cameraMover;
    [SerializeField] StageGenerator stageGenerator;
    [SerializeField] ItemSpawner itemSpawner;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] ScoreManager scoreManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Initialize()
    {
        var player = GameObject.FindFirstObjectByType<PlayerController>();
        if (player == null) throw new System.Exception();
        var playerTra = player.transform;
        
        stageGenerator.Initialize();
        positionSetuper.Initialize(playerTra);
        cameraMover.Initialize(playerTra);
        //itemSpawner.isInitialized = () => stageGenerator.isInitialize;
        itemSpawner.Initialize();
        enemySpawner.Initialize();
        scoreManager.Initialize();
    }
}
