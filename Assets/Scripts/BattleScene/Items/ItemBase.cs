using UnityEngine;
using Game.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Events;
using Game.SpawnableObj;
using System;
namespace Game.Item
{
    public interface IPickupedItem
    {
        bool isPicked { get;}
        UniTask OnPickUpItem(PlayerController player);
        UnityAction<Vector2Int> AfterPickUpedItem { get; set;}
        Vector2Int myMapNode { get; set;}
        void DestroyItem();
    }

    public abstract class ItemBase<TItem>: MonoBehaviour, IPickupedItem,ISpawnableObj where TItem : ItemBase<TItem>
    {
        protected ItemMover<TItem> itemMover;
        [SerializeField] ItemAudioDatas itemAudioDatas;
        protected ItemAudioDatas _itemAudioDatas => itemAudioDatas;
        public bool isPicked { get; protected set; } = false;
        public Vector2Int myMapNode { get; set; }
        public UnityAction<Vector2Int> AfterPickUpedItem { get; set; }

        public GameObject ownerObj => gameObject;

        public void DestroyItem() => Destroy(this.gameObject);

        protected ItemAudioHelper<TItem> itemAudioHelper;
        public abstract UniTask OnPickUpItem(PlayerController player);
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        protected abstract void Initialize();

        protected virtual async void Start()
        {
            Initialize();
            try
            {
                await UniTask.WaitUntil(() => itemMover != null, cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException) { return; }
            itemMover.StartInfinityAction().Forget();
        }        
        // Update is called once per frame
        void Update()
        {

        }
    }
}

