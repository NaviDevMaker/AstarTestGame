using Cysharp.Threading.Tasks;
using Game.Player;
using Unity.VisualScripting;
using UnityEngine;

public class BattleStateManager : MonoBehaviour
{
    public static BattleStateManager Instance { get; private set; }
    enum BattleState
    { 
       Proccesing,
       End
    }

    BattleState currentBattleState;
    public bool isEndBattle = false;
    private void Awake() => Instance = this;
    // Update is called once per frame
    private void Update()
    {
        if (isEndBattle) return;
        if(currentBattleState == BattleState.End) isEndBattle = true;
    }
    public void Initialize(PlayerController player)
    {
        currentBattleState = BattleState.Proccesing;
        player.OnDeadAction += async (_) =>
        {
            await UniTask.CompletedTask;
            currentBattleState = BattleState.End;
        };
    }
}
