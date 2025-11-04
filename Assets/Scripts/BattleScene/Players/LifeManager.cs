using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Game.Player
{
    public class LifeManager : MonoBehaviour,IAssetSetter
    {
        [SerializeField] PlayerController player;
        [SerializeField] Sprite noLifeSprite;
        [SerializeField] Sprite lifeSprite;
        Image lifeImagePrefab;
        Dictionary<Image, bool> lifeDic = new Dictionary<Image, bool>();
        public async UniTask GetAsset()
        {
            var imageObj = await GetAssetsMethods.GetAsset<GameObject>("Prefabs/LifeImage");
            if (imageObj == null) throw new System.Exception();
            lifeImagePrefab = imageObj.GetComponent<Image>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await  GetAsset();
            SetLifeImages();
        }
        // Update is called once per frame
        void Update()
        {

        }
        void SetLifeImages()
        {
            var lifeCount = player.playerStatusData.Life;
            for (int i = 0; i < lifeCount; i++)
            {
                var lifeImage = Instantiate(lifeImagePrefab);
                lifeImage.transform.SetParent(transform);
                var amount = 1.5f;
                lifeImage.transform.localScale = Vector3.one * amount;
                SetSprite(lifeImage, lifeSprite);
                lifeDic[lifeImage] = true;
            }
        }

        void SetSprite(Image image, Sprite newSprite) => image.sprite = newSprite;
        void ReduceLife()
        {

        }
    }
}


