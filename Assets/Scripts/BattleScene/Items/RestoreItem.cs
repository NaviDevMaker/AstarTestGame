using UnityEngine;
using Game.Player;
using Cysharp.Threading.Tasks;
namespace Game.Item.RestoreItem
{
    public class RestoreItem : ItemBase<RestoreItem>
    {
        public override async UniTask OnPickUpItem(PlayerController player)
        {
            Debug.Log(itemMover);
            isPicked = true;
            await itemMover.OnStartMove(player);
            LifeManager.Instance.RestoreLife();
            player.currentLife++;
            DestroyItem();
        }
        protected override void Initialize()
        {
            itemMover = new ItemMover<RestoreItem>(this);
            Debug.Log($"èâä˙âªÇ∑ÇÈÇ◊,{itemMover}");
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
        }
    }
}

