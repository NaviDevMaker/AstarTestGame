using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Player;

namespace Game.Item
{
    public interface IItemMoveEffect
    {
        UniTask OnStartMove(PlayerController player);
    }

    public class ItemMover : IAssetSetter,IItemMoveEffect
    {
        ItemMoveSetting itemMoveSetting;
        public async UniTask GetAsset() => itemMoveSetting = await GetAssetsMethods.GetAsset<ItemMoveSetting>("Datas/ItemData/ItemMoveSetting");

        public UniTask OnStartMove(PlayerController player)
        {
            throw new System.NotImplementedException();
        }

        //protected async UniTask ItemMoveAction(PlayerController player)
        //{
        //    try
        //    {


        //        transform.SetParent(player.transform);

        //    }

        //}

        //async UniTask GetMoveTask(PlayerController player, MoveInfo.Move move)
        //{
        //    var offset = Vector3.up * amount;
        //    var targetPos = player.transform.position + offset;
        //    while ((targetPos - transform.position).magnitude <= 0.01f)
        //    {
        //        var move = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveInfo.MoveSpeed);
        //        transform.position = move;
        //        await UniTask.Yield(cancellationToken: player.GetCancellationTokenOnDestroy());
        //    }
        //}

    }


}


