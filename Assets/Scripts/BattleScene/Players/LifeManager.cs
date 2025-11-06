using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;
namespace Game.Player
{
    public class LifeManager : MonoBehaviour,IAssetSetter
    {
        public static LifeManager Instance { get; private set; } = null;
        [SerializeField] PlayerController player;
        [SerializeField] Sprite noLifeSprite;
        [SerializeField] Sprite lifeSprite;
        Image lifeImagePrefab;
        Dictionary<Image, bool> lifeDic = new Dictionary<Image, bool>();
        Dictionary<Image, CancellationTokenSource> ctsDic = new Dictionary<Image, CancellationTokenSource>();
        Dictionary<Image, Vector3> scaleDic = new Dictionary<Image, Vector3>();
        public async UniTask GetAsset()
        {
            var imageObj = await GetAssetsMethods.GetAsset<GameObject>("Prefabs/LifeImage");
            if (imageObj == null) throw new System.Exception();
            lifeImagePrefab = imageObj.GetComponent<Image>();
        }
        void Awake()
        {
            if (Instance == null) Instance = this;
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
            if (Input.GetKeyDown(KeyCode.Space)) ReduceLife();
            else if (Input.GetKeyDown(KeyCode.X)) RestoreLife();
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
                var originalScale = lifeImage.transform.localScale;
                SetSprite(lifeImage, lifeSprite);
                lifeDic[lifeImage] = true;
                ctsDic[lifeImage] = null;
                scaleDic[lifeImage] = originalScale;
            }
        }
        void SetSprite(Image image, Sprite newSprite) => image.sprite = newSprite;
        async UniTask ChangeLife(bool isReducing)
        {
            var values = lifeDic.Values.ToList();
            var index = isReducing ? values.FindLastIndex(v => v)
                        : values.FindIndex(v => !v);
            if (index == -1) return;
            var keys = lifeDic.Keys.ToList();
            var targetImage = keys[index];
            
            lifeDic[targetImage] = isReducing ? false : true;
            var currentCts = ctsDic[targetImage];
            currentCts?.Cancel();
            var newCts = new CancellationTokenSource();
            ctsDic[targetImage] = newCts;
            var currentTargetScale = default(Vector3);
            try
            {
                var scaleupTask = GetScaleTask(targetImage, true, newCts,out currentTargetScale);
                await scaleupTask;
                var scaleDownTask = GetScaleTask(targetImage, false, newCts, out currentTargetScale);
                await scaleDownTask;
            }
            catch(OperationCanceledException){}
            finally { targetImage.transform.localScale = currentTargetScale;}
            var targetSprite = isReducing ? noLifeSprite : lifeSprite;
            SetSprite(targetImage, targetSprite);
        }
        UniTask GetScaleTask(Image targetImage,bool isUp,CancellationTokenSource cts,out Vector3 targetScale)
        {   
            var duration = 0.1f;
            var originalScale = scaleDic[targetImage];
            targetScale = isUp
                              ? new Func<Vector3>(() =>
                              {
                                  var amount = 3.0f;
                                  return originalScale * amount;
                              })()
                              : originalScale;
            var scaleSet = new Vector3TweenSetup(targetScale, duration);
            return targetImage.gameObject.Scaler(scaleSet).ToUniTask(cancellationToken:cts.Token);
        }
        void ReduceLife() => ChangeLife(true).Forget();
        public void RestoreLife() => ChangeLife(false).Forget();
    }
}


