using UnityEngine;

namespace Game.Enemy
{
    [CreateAssetMenu]
    public class EnemyAudioDatas : ScriptableObject
    {
        [Header("Audio clips")]
        [SerializeField] AudioClip moveAudio;
        [SerializeField] AudioClip deathAudio;

        [Header("Hearable Distance")]
        [SerializeField, Range(10, 30)] float hearableDistance;
        public AudioClip MoveAudio  => moveAudio;
        public AudioClip DeathAudio  => deathAudio;
        public float HearableDistance => hearableDistance; 
    }
}


