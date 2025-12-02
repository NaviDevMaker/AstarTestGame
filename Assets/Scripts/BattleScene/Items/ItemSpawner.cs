using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Stage;
using Game.Item;
using Game.SpawnableObj;
namespace Game.Spawner
{
    public class ItemSpawner : MonoBehaviour,ISpawner,ISetUper
    {
        SpawnHelper<ItemGenerateSetting> spawnHelper;
        public SpawnerType spawnerType => SpawnerType.Item;

        public bool IsSetUped { get; set; } = false;

        // Update is called once per frame
        void Update()
        {
            if (spawnHelper == null) return;
            //if (itemGenerateSetting == null) return;// || !isInitialized()
            if (spawnHelper.IsReachedSpawnTime() && IsSetUped)
            {
                if (spawnHelper.IsSpawnable(out var node)) Spawn(node.x, node.y);
            }
        }
        public void Initialize() => spawnHelper = new SpawnHelper<ItemGenerateSetting>(spawnerType);

        public void Spawn(int targetX,int targetY)
        {
            var mapPositionInfo = new MapPositionInfo(targetX,targetY);
            var pos = StageMethods.GetTargetNodePos(mapPositionInfo);
            var spawnableObj = GetTargetObj();
            var prafabObj = spawnableObj.ownerObj;
            var item = Instantiate(prafabObj, pos, prafabObj.transform.rotation);
            if (item.TryGetComponent<IPickupedItem>(out var pickupedItem)) pickupedItem.AfterPickUpedItem
                                      += ReturnNoItemStatus;
            spawnHelper.occupyMap[targetX, targetY] = 1;
            var node = new Vector2Int(targetX, targetY);
            pickupedItem.myMapNode = node;
           
        }
        public void ReturnNoItemStatus(Vector2Int itemMapNode)
        {
            var x = itemMapNode.x;
            var y = itemMapNode.y;
            spawnHelper.occupyMap[x, y] = 0;
        }
        public ISpawnableObj GetTargetObj()
        {
            var prefabSetting = spawnHelper.prefabGenerateSetting;
            var itemLength = prefabSetting.Prefabs.Count;
            //prefabSetting.Prefabs.ForEach(p => Debug.Log($"ƒvƒŒ‚Ó‚Ÿ‚Ô‚Ì–¼‘O : {p}"));
            var r = UnityEngine.Random.Range(0, itemLength);
            return prefabSetting.Prefabs[r];
        }
    }
}



