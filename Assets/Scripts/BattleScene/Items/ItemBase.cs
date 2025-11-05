using UnityEngine;
using Game.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
namespace Game.Item
{
    public interface IPickupedItem
    {
        void OnPickUpItem(PlayerController player);
        void DestroyItem();
    }

    public abstract class ItemBase : MonoBehaviour, IPickupedItem
    {
        public void DestroyItem() => Destroy(this.gameObject);
        public abstract void OnPickUpItem(PlayerController player);
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        // Update is called once per frame
        void Update()
        {

        }
      
    }
}

