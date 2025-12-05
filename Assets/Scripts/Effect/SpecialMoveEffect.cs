using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.Effect.SpecialMove
{
    public class SpecialMoveEffect:IEffect,IAssetSetter
    {
        public SpecialMoveEffect() { GetAsset().Forget(); }

        public float destroyTime => 3.0f;

        public ParticleSystem effect { get; private set;}

        public async UniTask AutoDestroy(ParticleSystem particle)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(destroyTime));
            if (particle != null) UnityEngine.Object.Destroy(particle.gameObject);
        }
        public async UniTask GetAsset()
        {
            var obj = await GetAssetsMethods.GetAsset<GameObject>("Effects/SpecialMoveEffect");
            if (obj == null) throw new Exception();
            effect = obj.GetComponent<ParticleSystem>();
        }

        public ParticleSystem GetEffect(Vector3 pos, Quaternion rot = default, Transform parent = null, bool autoDestroy = true)
        {
            var spawnedEffect = UnityEngine.Object.Instantiate(effect,pos,rot,parent);
            if(autoDestroy) AutoDestroy(spawnedEffect).Forget();
            return spawnedEffect;
        }
    }
}


