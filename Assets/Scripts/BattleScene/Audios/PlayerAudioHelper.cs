using UnityEngine;

namespace Game.Player
{
    public class PlayerAudioHelper
    {
        public PlayerAudioHelper(PlayerAudioDatas playerAudioDatas, AudioSource audioSource)
        {
            this.playerAudioDatas = playerAudioDatas;
            this.audioSource = audioSource;
        }

        PlayerAudioDatas playerAudioDatas;
        AudioSource audioSource;

        enum PlayerAudioType
        {
            Foot,
            Hitted,
            Attack,
            Death
        }

        public void PlayFootAudio()
        {
            audioSource.clip = GetTargetAudioClip(PlayerAudioType.Foot);
            audioSource.Play();
        }
        public void PlayHittedAudio() => audioSource.PlayOneShot(GetTargetAudioClip(PlayerAudioType.Hitted));

        public void PlayAttackAudio() => audioSource.PlayOneShot(GetTargetAudioClip(PlayerAudioType.Attack));
        public void PlayDeathAudio() => audioSource.PlayOneShot(GetTargetAudioClip(PlayerAudioType.Death));

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


