using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.Effect.Hit
{
    public class HitEffect : IAssetSetter, IEffect
    {
        public HitEffect() { GetAsset().Forget(); }
        public float destroyTime => 1.0f;
        public ParticleSystem effect { get; private set; }
        public async UniTask AutoDestroy(ParticleSystem particle)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(destroyTime)
                               ,cancellationToken:particle.GetCancellationTokenOnDestroy());
            UnityEngine.Object.Destroy(particle.gameObject);
        }
        public async UniTask GetAsset()
        {
            var hitObj = await GetAssetsMethods.GetAsset<GameObject>("Prefabs/Effects/HitEffect");
            if (hitObj == null) throw new System.NullReferenceException("The object is null!!");
            effect = hitObj.GetComponent<ParticleSystem>();
        }
        public ParticleSystem GetEffect(Vector3 pos, Quaternion rot = default, Transform parent = null, bool autoDestroy = true)
        {
            var effect = UnityEngine.Object.Instantiate(this.effect, pos, rot, parent);
            if (autoDestroy) AutoDestroy(effect).Forget();
            return effect;
        }
        public void SpawnHitEffect(GameObject target)
        {
            var parent = target.transform;
            var pos = parent.position;
            var effect = GetEffect(pos,parent:parent);
            effect.Play();
        }
    }
}

