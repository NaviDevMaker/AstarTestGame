using Cysharp.Threading.Tasks;
using Game.Effect.Smoke;
using UnityEngine;

namespace Game.Effect
{
    public interface IEffect
    {
        float destroyTime { get; }
        UniTask AutoDestroy(ParticleSystem particle);
    }

    public class EffectManager : SigletonMonobehaiver<EffectManager>
    {
       public  SmokeEffect smokeEffect { get; private set;}

        private void Start()
        {
            Initialize();
        }
        void Initialize()
        {
            smokeEffect = new SmokeEffect();
        }     
    }

}

