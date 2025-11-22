using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;


namespace Game.Enemy
{
    public class EnemyActionHelper<TEnemy>:IAssetSetter where TEnemy : EnemyController
    {
        public EnemyActionHelper(TEnemy owner)
        {
            this.owner = owner;
            token = owner.GetCancellationTokenOnDestroy();
            GetAsset().Forget();
        }

        TEnemy owner;
        EnemyActionFieldDatas actionFieldDatas;
        CancellationToken token;
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

        public async UniTask FadeInAction(float length)
        {
            var mat = owner.meshMat;
            var color = mat.color;
            color.a = 1.0f;
            mat.color = color;
            var targetAlpha = 0f;
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
            await tween.ToUniTask(cancellationToken:token);
        }
        public async UniTask GetAsset()
        {
            actionFieldDatas = await GetAssetsMethods.GetAsset<EnemyActionFieldDatas>("Datas/Enemy/EnemyActionFieldData");
            if (actionFieldDatas == null) throw new System.Exception("エネミーアクションデータがありません!!");
        }
    }
}

