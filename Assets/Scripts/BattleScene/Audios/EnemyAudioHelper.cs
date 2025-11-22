using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Player;
using System;
namespace Game.Enemy
{
    public class EnemyAudioHelper<TEnemy> where TEnemy : EnemyController
    {
        TEnemy owner;
        EnemyAudioDatas audioDatas;
        AudioSource audioSource;

        float maxDistance = 0f;
        PlayerController player;

        float minVolume = 0f;
        float maxVolume = 1.0f;
        float maxPicth = 3f;

        enum EnemyAudioType
        {
            Move,
            Death
        }

        public EnemyAudioHelper(TEnemy owner,AudioSource audioSource,EnemyAudioDatas enemyAudioDatas)
        {
            this.owner = owner;
            this.audioSource = audioSource;
            this.audioDatas = enemyAudioDatas;
            maxDistance = enemyAudioDatas.HearableDistance;
            player = GameObject.FindFirstObjectByType<PlayerController>();
            audioSource.spatialBlend = 1.0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            StartChangeVolume().Forget();
        }
        public void PlayDeathAudio(float targetLength)
        {
            audioSource.Stop();
            var targetClip = GetTargetAudioClip(EnemyAudioType.Death);
            PitchChange(targetClip, targetLength);
            audioSource.PlayOneShot(targetClip);
        }
        public void PlayMoveAudio()
        {
            if (!audioSource.loop) audioSource.loop = true;
            audioSource.clip = GetTargetAudioClip(EnemyAudioType.Move);
            audioSource.Play();
        }
        void ChangeAudioVolume()
        {
            var distance = (player.transform.position - owner.transform.position).magnitude;
            var lerp = Mathf.Clamp01(1 - distance / maxDistance);
            audioSource.volume = Mathf.Lerp(minVolume,maxVolume,lerp);
        }

        async UniTask StartChangeVolume()
        {
            var delayTime = 0.1f;
            var token = owner.GetCancellationTokenOnDestroy();
            while(!owner.isDead)
            {
                ChangeAudioVolume();
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime)
                                   ,cancellationToken:token);
            }
        }
        public void PitchChange(AudioClip audioClip,float targetLength)
        {
            var audioLength = audioClip.length;
            var pitch = Mathf.Min(audioLength / targetLength,maxPicth);
            audioSource.pitch = pitch;
        }

        AudioClip GetTargetAudioClip(EnemyAudioType audioType)
        {
            return audioType switch
            {
                EnemyAudioType.Move => audioDatas.MoveAudio,
                EnemyAudioType.Death => audioDatas.DeathAudio,
                _ => default
            };
        }
    }
}

