using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using Game.Effect;
using System;
using System.Collections.Generic;
using Game.Stage;
using Game.Player;
using Unity.VisualScripting;


namespace Game.Enemy
{
    public class EnemyActionHelper<TEnemy>:IAssetSetter where TEnemy : EnemyController
    {
        public EnemyActionHelper(TEnemy owner)
        {
            this.owner = owner;
            token = owner.GetCancellationTokenOnDestroy();
            var stageGenerater = StageGenerator.Instance;
            var map = stageGenerater.map;
            var defaultPos = stageGenerater.defaultPosition;
            aStarPathFinder = new AStarPathFinder(map,defaultPos);
            targetPlayer = PlayerController.instance;
            moveSpeed = owner._enemyStatusData.MoveSpeed;
            GetAsset().Forget();
        }

        TEnemy owner;
        PlayerController targetPlayer;
        EnemyActionFieldDatas actionFieldDatas;
        CancellationToken token;
        AStarPathFinder aStarPathFinder;
        float moveSpeed = 0f;
        public async UniTask StartTranslusentAction()
        {
            await UniTask.WaitUntil(() => actionFieldDatas != null,cancellationToken:token);
            var baseAlpha = 0.5f;
            var changeSpeed = actionFieldDatas.ChangeSpeed;

            while(!owner.isDead)
            {
                var alpha = (Mathf.Sin(Time.time * changeSpeed  * Mathf.Deg2Rad)) * baseAlpha + baseAlpha;
                var color = owner.meshMat.color;
                color.a = alpha;
                owner.meshMat.color = color;
                await UniTask.Yield(cancellationToken: token);
            }
        }
        public async UniTask MoveUpAction()
        {
            var upAmount = actionFieldDatas.UpAmount;
            var targetPos = owner.transform.position + Vector3.up * upAmount;
            var duration = actionFieldDatas.UpDuration;
            await owner.gameObject.Mover(new Vector3TweenSetup(targetPos, duration))
                                        .ToUniTask(cancellationToken:token);
        }
        public async UniTask FadeAction(float length,float startAlpha,float targetAlpha)
        {
            try
            {
                var mat = owner.meshMat;
                var color = mat.color;
                color.a = startAlpha;
                mat.color = color;
                var tween = DOTween.To(
                    () => mat.color.a,
                    alpha =>
                    {
                        var newColor = mat.color;
                        newColor.a = alpha;
                        mat.color = newColor;
                    },
                    targetAlpha,
                    length
                    );
                await tween.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException) { throw; }
        }
        public async UniTask GetAsset()
        {
            actionFieldDatas = await GetAssetsMethods.GetAsset<EnemyActionFieldDatas>("Datas/Enemy/EnemyActionFieldData");
            if (actionFieldDatas == null) throw new System.Exception("エネミーアクションデータがありません!!");
        }

        public async UniTask SpawnAction()
        {
            try
            {
                var pos = owner.transform.position;
                var parent = owner.transform;
                var smokeEffect = EffectManager.Instance.smokeEffect.GetSmokeEffect(pos, parent: parent);
                var main = smokeEffect.main;
                var simulationSpeed = main.simulationSpeed;
                var rawDuration = main.duration;
                var length = rawDuration / simulationSpeed;
                smokeEffect.Play();
                await FadeAction(length,startAlpha:0f,targetAlpha:1.0f);
            }
            catch (OperationCanceledException) { throw; }
        }

        public async UniTask ChaseLooper(CancellationTokenSource chaseEndCts)
        {
            CancellationTokenSource moveCts = null;
            var distBasedSqr = actionFieldDatas.DistBasedSqr;
            //var isChangingPathes = false;
            try
            {
                while (!chaseEndCts.IsCancellationRequested)
                {
                    // ★ 前の移動をキャンセル
                    moveCts?.Cancel();
                    moveCts?.Dispose();
                    moveCts = new CancellationTokenSource();

                    var worldStart = owner.transform.position;
                    var worldGoal = targetPlayer.transform.position;

                    //if (isChangingPathes)
                    //{
                    //    await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: chaseEndCts.Token);
                    //    isChangingPathes = false;
                    //}
                    var pathes = await aStarPathFinder.SearchPathAsync(
                        worldStart,
                        worldGoal,
                        chaseEndCts.Token
                    );
                    if (pathes == null || pathes.Count == 0) continue;
                    // 移動開始
                    MoveToTarget(pathes, moveCts).Forget();

                    await UniTask.WaitUntil(
                        () =>
                        {
                            // 1.5f 以上動いたら
                            float dist = (targetPlayer.transform.position - worldGoal).sqrMagnitude;
                            return dist > distBasedSqr;
                            //var isContinuable = dist > 1.5f;
                            //if(isContinuable) isChangingPathes = true;
                            //return isChangingPathes;
                        },
                        cancellationToken: chaseEndCts.Token
                    );
                }
            }
            catch (OperationCanceledException) { }
        }

        async UniTask MoveToTarget(List<Vector3> pathes,CancellationTokenSource moveCts)
        {
            
            try
            {
                foreach (var path in pathes)
                {
                    //今までなんとなくで使ってたからよくない、これsqrは距離の２乗値でこのような比較の時には実際の距離は必要ない
                    //だからsqrを使う、magunitudeだと√magunitudeを内部的に計算するから遅くなる
                    //ちなみにこの0.2fは距離で言うと0.45fでそれを２乗した値、つまり0.45fｍ未満になるまでという意味になる、だからそこは任意
                    while((path - owner.transform.position).sqrMagnitude > 0.1f)
                    {
                        moveCts.Token.ThrowIfCancellationRequested();
                        var move = Vector3.MoveTowards(owner.transform.position, path, moveSpeed * Time.deltaTime);
                        var dir = (move - owner.transform.position).normalized;
                        if(dir != Vector3.zero) owner.transform.rotation = Quaternion.LookRotation(dir);
                        owner.transform.position = move;
                        await UniTask.Yield();
                    }
                    Debug.Log($"Pathは{path}");
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }          
        }
    }
}

