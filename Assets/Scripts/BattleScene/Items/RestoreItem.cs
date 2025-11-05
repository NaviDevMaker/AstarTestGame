using UnityEngine;
using Game.Player;
namespace Game.Item.RestoreItem
{
    public class RestoreItem : ItemBase
    {
        public override void OnPickUpItem(PlayerController player)
        {
            LifeManager.Instance.RestoreLife();
            player.currentLife++;
            DestroyItem();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

