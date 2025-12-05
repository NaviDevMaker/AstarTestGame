using Game.Player;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Game.Icon
{
    public class BattleIconManager : MonoBehaviour
    {
        [SerializeField] AttackIconManager attackIconManager;
        [SerializeField] SpecialMoveIconManager specialMoveIconManager;
        public void Initialize(PlayerController player)
        {
            attackIconManager.Initialize(player);
            specialMoveIconManager.Initialize(player);  
        }
    }
}


