using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Player;
using System;
using System.Threading;
using DG.Tweening;
namespace Game.Item
{
    public interface IItemMoveEffect
    {
        UniTask OnStartMove(PlayerController player);
    }

    public class ItemMover<TItem> : IAssetSetter, IItemMoveEffect where TItem : ItemBase<TItem>
    {
        public ItemMover(TItem owner) => Initialize(owner).Forget();
        TItem owner;
        static ItemMoveSetting itemMoveSetting = null;
        CancellationTokenSource cts = new CancellationTokenSource();
        async UniTask Initialize(TItem owner)
        {
            this.owner = owner;
            if(itemMoveSetting == null) await GetAsset();
            if (itemMoveSetting == null) throw new System.Exception("アイテムセッティングがnullです");
        }
        public async UniTask GetAsset() => itemMoveSetting = await GetAssetsMethods.GetAsset<ItemMoveSetting>("Datas/ItemData/ItemMoveSetting");

        public async UniTask OnStartMove(PlayerController player) => await ItemMoveAction(player);
        protected async UniTask ItemMoveAction(PlayerController player)
        {
            cts?.Cancel();
            cts?.Dispose();
            try
            {
                var fowardTask = GetMoveTask(player, MoveInfo.MoveFoward);
                await fowardTask();
                var upTask = GetMoveTask(player, MoveInfo.Up);
                await upTask();
                var downTask = GetMoveTask(player, MoveInfo.Down);
                await downTask();
            }
            catch (OperationCanceledException) { }
        }
        Func<UniTask> GetMoveTask(PlayerController player, MoveInfo moveInfo)
        {
            return async() =>
            {
                try
                {
                    var token = player.GetCancellationTokenOnDestroy();
                    var targetPos = GetTargetPos(player, moveInfo);
                    while ((targetPos - owner.transform.position).magnitude > 0.1f)
                    {
                        token.ThrowIfCancellationRequested();
                        var move = Vector3.MoveTowards(owner.transform.position, targetPos, Time.deltaTime * itemMoveSetting.MoveSpeed);
                        owner.transform.position = move;
                        await UniTask.Yield(cancellationToken:token);
                        targetPos = GetTargetPos(player, moveInfo);
                    }
                }
                catch (OperationCanceledException) { throw; }
        　　};
        }
        Vector3 GetOffset(MoveInfo moveInfo)
        {
            return moveInfo switch
            { 
                MoveInfo.MoveFoward => Vector3.up * itemMoveSetting.UpOffset,
                MoveInfo.Up => Vector3.up * (itemMoveSetting.UpOffset + itemMoveSetting.FurtherUpOffset),
                MoveInfo.Down => Vector3.down * itemMoveSetting.DownOffset,
                _=> default
            };
        }
        Vector3 GetTargetPos(PlayerController player,MoveInfo moveInfo)
        {
            var offset = GetOffset(moveInfo);
            var playerPos = player.transform.position;
            return playerPos + offset;
        }
        public async UniTask StartInfinityAction()
        {
            try
            {
                await UniTask.WaitUntil(() => itemMoveSetting != null, cancellationToken: cts.Token);
                RotateItem().Forget();
                FloatItem().Forget();
            }
            catch (OperationCanceledException) { }
        }
        async UniTask RotateItem()
        {

            Debug.Log(itemMoveSetting);
            var duraion = itemMoveSetting.RotateDuration;
            var targetRot = new Vector3(0f, 360f, 0f);
            var ease = Ease.InOutSine;
            var rotateSet = new Vector3TweenSetup(targetRot, duraion, ease);
            var rotateTask = owner.gameObject.Roter(rotateSet, RotateMode.FastBeyond360)
                              .SetLoops(-1, LoopType.Restart)
                              .ToUniTask(tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait
                                         ,cancellationToken:cts.Token);
            try { await rotateTask;}
            catch (OperationCanceledException) { }
        }

        async UniTask FloatItem()
        {
            Debug.Log(itemMoveSetting);
            var t = 0f;
            var floatSpeed = itemMoveSetting.FloatSpeed;
            var absOffset = itemMoveSetting.FloatOffset;
            var startY = owner.transform.position.y + absOffset;
            try
            {
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    t += Time.deltaTime * floatSpeed;
                    var targetY = startY + Mathf.Sin(t) * absOffset;//Sin単位円的な大きさだから絶対値で最大値が必要
                    owner.transform.position = new Vector3(owner.transform.position.x, targetY, owner.transform.position.z);
                    await UniTask.WaitForEndOfFrame(cancellationToken: cts.Token); 
                }
            }
            catch (OperationCanceledException) { }        
        }
    }
}


