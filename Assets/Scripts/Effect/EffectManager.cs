using Cysharp.Threading.Tasks;
using Game.Effect.Heal;
using Game.Effect.Hit;
using Game.Effect.Smoke;
using Game.Effect.SpecialMove;
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
    public class EffectManager : SingletonMonobehaviour<EffectManager>
    {
       public  SmokeEffect smokeEffect { get; private set;}
       public HitEffect hitEffect { get; private set; }
       public HealEffect healEffect { get; private set; }
        public SpecialMoveEffect specialMoveEffect { get; private set; }
        private void Start() => Initialize();
        void Initialize()
        {
            smokeEffect = new SmokeEffect();
            hitEffect = new HitEffect();
            healEffect = new HealEffect();  
            specialMoveEffect = new SpecialMoveEffect();
        }     
    }
}

