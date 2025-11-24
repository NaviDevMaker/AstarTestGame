using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.Effect.Smoke
{
    public class SmokeEffect : IAssetSetter,IEffect
    {
        public SmokeEffect() { GetAsset().Forget(); }
        ParticleSystem smokeEffect;

        public float destroyTime => 2.0f;

        public async UniTask GetAsset()
        {
            var smokeObj = await GetAssetsMethods.GetAsset<GameObject>("Prefabs/Effects/SpawnSmoke");
            if (smokeObj == null) throw new System.NullReferenceException("The object is null!!");
            smokeEffect = smokeObj.GetComponent<ParticleSystem>();
        }

        public ParticleSystem GetSmokeEffect(Vector3 pos,Quaternion rot = default,Transform parent = null,bool autoDestroy = true)
        {
            var effect = UnityEngine.Object.Instantiate(smokeEffect, pos, rot, parent);
            if (autoDestroy) AutoDestroy(effect).Forget();
            return effect;
        }

        public async UniTask AutoDestroy(ParticleSystem particle)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(destroyTime));
            UnityEngine.Object.Destroy(particle.gameObject);
        }
    }

}

