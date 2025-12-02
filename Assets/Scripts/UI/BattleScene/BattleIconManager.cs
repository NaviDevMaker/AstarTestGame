using Game.Player;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Game.Icon
{
    public class BattleIconManager : MonoBehaviour
    {
        [SerializeField] AttackIconManager attackIconManager;
        public void Initialize(PlayerController player)
        {
            attackIconManager.Initialize(player);
        }
    }
}


