using UnityEngine;
using Game.Player;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using Game.Effect;
using System;
using System.Threading;
namespace Game.Item.RestoreItem
{
    public class RestoreItem : ItemBase<RestoreItem>
    {
        [SerializeField] int restoreAmount;
        ParticleSystem currentParticle;
        public override async UniTask OnPickUpItem(PlayerController player)
        {
            Debug.Log(itemMover);
            isPicked = true;
            await itemMover.OnStartMove(player);
            Func<UniTask> effectAction = EffectAction(player);
            LifeManager.Instance.RestoreLife(restoreAmount,() => effectAction()).Forget();

            var targetLife = player.currentLife + restoreAmount;
            if(targetLife > player.playerStatusData.Life) targetLife = player.playerStatusData.Life;
            player.currentLife = targetLife;
            itemAudioHelper.PlayPickUpAudio();
            DestroyItem();
        }
        protected override void Initialize()
        {
            itemMover = new ItemMover<RestoreItem>(this);
            itemAudioHelper = new ItemAudioHelper<RestoreItem>(this,_itemAudioDatas);
            Debug.Log($"èâä˙âªÇ∑ÇÈÇ◊,{itemMover}");
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
         protected override void Start()
         {
            base.Start();
         }
         Func<UniTask> EffectAction(PlayerController player)
         {
            return async () =>
            {
                if (currentParticle != null)
                {
                    var duration = 0.5f;
                    await UniTask.Delay(TimeSpan.FromSeconds(duration));
                    currentParticle = null;
                }
                var offsetY = Vector3.up * 0.1f;
                var pos = player.transform.position + offsetY;
                var parent = player.transform;
                var effect = EffectManager.Instance.healEffect.GetEffect(pos, parent: parent);
                currentParticle = effect;
                effect.Play();
            };            
        }
    }
}

