using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu]
    public class PlayerAudioDatas : ScriptableObject
    {
        [SerializeField] AudioClip hittedAudio;
        [SerializeField] AudioClip attackAudio;
        [SerializeField] AudioClip footAudio;
        [SerializeField] AudioClip deathAudio;
        [SerializeField] AudioClip specialMoveAudio;
        public AudioClip HittedAudio => hittedAudio; 
        public AudioClip DeathAudio => deathAudio;
        public AudioClip FootAudio => footAudio;
        public AudioClip AttackAudio => attackAudio;
        public AudioClip SpecialMoveAudio => specialMoveAudio;
    }
}


