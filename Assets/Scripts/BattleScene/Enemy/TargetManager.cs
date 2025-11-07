using UnityEngine;
using Game.Player;
using Cysharp.Threading.Tasks;
public class TargetManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    class PlayerInfo
    {
        public PlayerController _player { get; set; }
        public float detectRange { get; set; }
        public float thereHold { get; set; }
    }
    PlayerInfo playerInfo;
    public static TargetManager Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start() => Initialize();

    void Initialize()
    {
        var attackAngle = player.playerStatusData.AttackAngle;
        var thereHold = Mathf.Cos(attackAngle * 0.5f * Mathf.Deg2Rad);
        playerInfo = new PlayerInfo
        {
            _player = this.player,
            detectRange = player.playerStatusData.DetectRange,
            thereHold = thereHold
        };
    }
    public void SetCurrentTarget(IEnemy targetEnemy)
    {
        var player = playerInfo._player;
        var currentTarget = player.currentTarget;
        Debug.Log(IsTargetable(targetEnemy));
        if (!IsTargetable(targetEnemy))
        {
            if(currentTarget == targetEnemy) player.currentTarget = null;
            return;
        }
        Debug.Log("ターゲットに設定します", gameObject);
        var playerPos = player.transform.position;
        playerPos = GetFlatPosition(playerPos);
        var newTargetEnemyPos = targetEnemy.enemyCollider.ClosestPoint(playerPos);
        newTargetEnemyPos = GetFlatPosition(newTargetEnemyPos);
        var newTargetDistance = Vector3.Distance(playerPos, newTargetEnemyPos);     
        if (newTargetDistance <= playerInfo.detectRange)
        {
            if(currentTarget == null) player.currentTarget = targetEnemy;
            else
            {
                var currentTargetPos = currentTarget.enemyCollider.ClosestPoint(playerPos);
                currentTargetPos = GetFlatPosition(currentTargetPos);
                var currentTargetDistance = Vector3.Distance(playerPos, currentTargetPos);
                if (newTargetDistance < currentTargetDistance) player.currentTarget = targetEnemy;
            }
        }
        else
        {
            if (player.currentTarget == targetEnemy) player.currentTarget = null;
        }
    }
    bool IsTargetable(IEnemy enemy)
    {
        var player = playerInfo._player;
        var playerFoward = player.transform.forward;
        playerFoward.y = 0f;
        playerFoward.Normalize();//ｙを０にした時点で長さが１のベクトルじゃなくなるから必要
        //方向の判定に傾きは考慮しないからyを0にする
        var flatPlayerPos = player.transform.position;
        flatPlayerPos.y = 0f;
        var closest = enemy.enemyCollider.ClosestPoint(flatPlayerPos);
        closest.y = 0f;
        var toEnemy = (closest - flatPlayerPos).normalized;
        var dot = Vector3.Dot(playerFoward, toEnemy);
        //Debug.Log($"thereHold: {playerInfo.thereHold}, dot: {dot}");
        return dot >= playerInfo.thereHold;
    }
    Vector3 GetFlatPosition(Vector3 position)
    {
        position.y = Terrain.activeTerrain.SampleHeight(position);
        return position;
    }
}
