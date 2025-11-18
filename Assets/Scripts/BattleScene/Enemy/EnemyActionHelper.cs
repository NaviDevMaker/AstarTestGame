using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.Enemy
{
    public class EnemyActionHelper<TEnemy> where TEnemy : EnemyController
    {
        public EnemyActionHelper(TEnemy owner)
        {
            this.owner = owner;
        }

        TEnemy owner;
        public async UniTask StartTranslusentAction()
        {
            var baseAlpha = 0.5f;
            var changeSpeed = 10.0f;

            //ƒeƒXƒg‚¾‚©‚çtrue
            while(true)
            {
                var alpha = (Mathf.Sin(Time.time * changeSpeed  * Mathf.Deg2Rad)) * baseAlpha + 0.5f;
                var color = owner.meshMat.color;
                color.a = alpha;
                owner.meshMat.color = color;
                await UniTask.Yield();
            }
        }
    }
}

