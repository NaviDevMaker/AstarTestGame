using UnityEngine;
using Game.Player;
using Game.Stage;
using Game.Spawner;
using Cysharp.Threading.Tasks;
using Game.Icon;
using Game.TextRenewer;

public interface ISetUper { bool IsSetUped { get;set;} }
public class BattleManager : MonoBehaviour
{
    [SerializeField] bool isTestOnly;

    [Header("–{”Ô—p‚Ë")]
    [SerializeField] BattleStartCounter battleStartCounter;
    [SerializeField] PosaitionSetUper positionSetuper;
    [SerializeField] CameraMover cameraMover;
    [SerializeField] StageGenerator stageGenerator;
    [SerializeField] ItemSpawner itemSpawner;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] BattleIconManager battleIconManager;
    [SerializeField] GhostAliveCounter ghostAliveCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(isTestOnly) Initialize().Forget();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public async UniTask Initialize()
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
        battleIconManager.Initialize(player);
        ghostAliveCounter.Initialize();
        if(!isTestOnly) await battleStartCounter.StartCountDown();
        itemSpawner.IsSetUped = true;
        enemySpawner.IsSetUped = true;
        player.IsSetUped = true;
    }
}
