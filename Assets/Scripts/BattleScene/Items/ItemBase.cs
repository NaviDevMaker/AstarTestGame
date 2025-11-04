using UnityEngine;

namespace Game.Item
{
    public interface IPickupedItem
    {
        void OnPickUpItem();
    }

    public abstract class ItemBase : MonoBehaviour,IPickupedItem
    {
        public abstract void OnPickUpItem();
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

