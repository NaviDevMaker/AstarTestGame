using UnityEngine;
using Game.Player;
public class BattleManager : MonoBehaviour
{
    [SerializeField] PosaitionSetUper positionSetuper;
    [SerializeField] CameraMover cameraMover;
    [SerializeField] StageGenerator stageGenerator;
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
        positionSetuper.Initialize(playerTra);
        cameraMover.Initialize(playerTra);
        stageGenerator.Initialize();
    }
}
