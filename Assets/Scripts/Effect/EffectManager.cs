using Cysharp.Threading.Tasks;
using Game.Effect.Hit;
using Game.Effect.Smoke;
using UnityEngine;

namespace Game.Effect
{
    public interface IEffect
    {
        float destroyTime { get; }
        UniTask AutoDestroy(ParticleSystem particle);

        ParticleSystem GetEffect(Vector3 pos, Quaternion rot = default, Transform parent = null, bool autoDestroy = true);
        ParticleSystem effect { get; }
    }

    public class EffectManager : SigletonMonobehaiver<EffectManager>
    {
       public  SmokeEffect smokeEffect { get; private set;}
       public HitEffect hitEffect { get; private set; }

        private void Start()
        {
            Initialize();
        }
        void Initialize()
        {
            smokeEffect = new SmokeEffect();
            hitEffect = new HitEffect();
        }     
    }

}

