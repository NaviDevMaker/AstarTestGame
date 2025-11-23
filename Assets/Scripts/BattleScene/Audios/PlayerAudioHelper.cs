using UnityEngine;

namespace Game.Player
{
    public class PlayerAudioHelper
    {
        public PlayerAudioHelper(PlayerAudioDatas playerAudioDatas, AudioSource sfxAudioSource,AudioSource footAudioSource)
        {
            this.playerAudioDatas = playerAudioDatas;
            this.sfxAudioSource = sfxAudioSource;
            this.footAudioSource = footAudioSource;
        }

        PlayerAudioDatas playerAudioDatas;
        AudioSource sfxAudioSource;
        AudioSource footAudioSource;

        enum PlayerAudioType
        {
            Foot,
            Hitted,
            Attack,
            Death
        }

        public void StartFootAudio()
        {
            footAudioSource.clip = GetTargetAudioClip(PlayerAudioType.Foot);
            footAudioSource.Play();
        }

        public void StopFootAudio() => footAudioSource.Stop();
        public void PlayHittedAudio() => sfxAudioSource.PlayOneShot(GetTargetAudioClip(PlayerAudioType.Hitted));

        public void PlayAttackAudio() => sfxAudioSource.PlayOneShot(GetTargetAudioClip(PlayerAudioType.Attack));
        public void PlayDeathAudio()
        {
            StopFootAudio();
            sfxAudioSource.PlayOneShot(GetTargetAudioClip(PlayerAudioType.Death));
        }

        AudioClip GetTargetAudioClip(PlayerAudioType audioType)
        {
            return audioType switch
            {
                PlayerAudioType.Foot => playerAudioDatas.FootAudio,
                PlayerAudioType.Attack => playerAudioDatas.AttackAudio,
                PlayerAudioType.Hitted => playerAudioDatas.HittedAudio,
                PlayerAudioType.Death => playerAudioDatas.DeathAudio,
                _=> default
            };
        }
    }
}


