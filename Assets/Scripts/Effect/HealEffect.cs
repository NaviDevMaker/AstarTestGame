using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.Effect.Heal
{
    public class HealEffect : IAssetSetter, IEffect
    {
        public HealEffect() { GetAsset().Forget(); }
        public float destroyTime => 1.0f;
        public ParticleSystem effect { get; private set;}
        public async UniTask AutoDestroy(ParticleSystem particle)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(destroyTime));
            UnityEngine.Object.Destroy(particle.gameObject);
        }
        public async UniTask GetAsset()
        {
            var effectObj = await GetAssetsMethods.GetAsset<GameObject>("Prefabs/Effects/HealEffect");
            if(effectObj == null) throw new System.NotImplementedException();
            effect = effectObj.GetComponent<ParticleSystem>();
        }
        public ParticleSystem GetEffect(Vector3 pos, Quaternion rot = default, Transform parent = null, bool autoDestroy = true)
        {
            var spawnedEffect = UnityEngine.Object.Instantiate(effect,pos,rot,parent);
            if(autoDestroy) AutoDestroy(spawnedEffect).Forget();
            return spawnedEffect;
        }
    }
}

