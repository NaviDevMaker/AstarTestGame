using UnityEngine;

namespace Game.Item
{
    [CreateAssetMenu]
    public class ItemAudioDatas : ScriptableObject
    {
        [SerializeField] AudioClip pickUpAudio;
        public AudioClip PickUpAudio => pickUpAudio;
    }
}

