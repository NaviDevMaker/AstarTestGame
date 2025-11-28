using Game.Spawner;
using UnityEngine;
using UnityEngine.UI;
namespace Game.Text
{
    //マジで依存関係聞かないでも理論だててかんがえられるようになってね、とりあえずUI
    //というただUIの更新をするところがゲームのシステムから参照されるのはよくないからね
    //出来ればロジック側はなにもUI側を知らないってのを意識して
    public class GhostAliveCounter : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Text ghostCountText;
        [SerializeField] EnemySpawner enemySpawner;
        int currentEnemyCount = 0;
        const string content = "Ghosts Alive:";
        public void Initialize()
        {
            ghostCountText.text = $"{content}{currentEnemyCount.ToString("D3")}";
            enemySpawner.OnChangeCount += UpdateText;
        }
        void UpdateText(bool isDeadEnemy)
        {
            currentEnemyCount = isDeadEnemy
                                ? --currentEnemyCount
                                : ++currentEnemyCount;
            ghostCountText.text = $"{content}{currentEnemyCount.ToString("D3")}";
        }
    }
}

