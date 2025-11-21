using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

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
        float changeSpeed = 0f;
        CancellationToken token;
        public async UniTask StartTranslusentAction()
        {
            await UniTask.WaitUntil(() => actionFieldDatas != null,cancellationToken:token);
            var baseAlpha = 0.5f;
            var changeSpeed = actionFieldDatas.ChangeSpeed;

            //テストだからtrue
            while(true)
            {
                var alpha = (Mathf.Sin(Time.time * changeSpeed  * Mathf.Deg2Rad)) * baseAlpha + baseAlpha;
                var color = owner.meshMat.color;
                color.a = alpha;
                owner.meshMat.color = color;
                await UniTask.Yield(cancellationToken: token);
            }
        }

        public async UniTask GetAsset()
        {
            actionFieldDatas = await GetAssetsMethods.GetAsset<EnemyActionFieldDatas>("Datas/Enemy/EnemyActionFieldData");
            if (actionFieldDatas == null) throw new System.Exception("エネミーアクションデータがありません!!");
        }
    }
}

