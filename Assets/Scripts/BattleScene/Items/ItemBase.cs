using UnityEngine;
using Game.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
namespace Game.Item
{
    public interface IPickupedItem
    {
        bool isPicked { get;}
        UniTask OnPickUpItem(PlayerController player);
        void DestroyItem();
    }

    public abstract class ItemBase<TItem>: MonoBehaviour, IPickupedItem where TItem : ItemBase<TItem>
    {
        protected ItemMover<TItem> itemMover;

        public bool isPicked { get; protected set; } = false;

        public void DestroyItem() => Destroy(this.gameObject);
        public abstract UniTask OnPickUpItem(PlayerController player);
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        protected abstract void Initialize();

        protected virtual async void Start()
        {
            Initialize();
            await UniTask.WaitUntil(() => itemMover != null,cancellationToken:this.GetCancellationTokenOnDestroy());
            itemMover.StartInfinityAction().Forget();
        }        
        // Update is called once per frame
        void Update()
        {

        }
    }
}

