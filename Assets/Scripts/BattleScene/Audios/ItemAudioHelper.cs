using UnityEngine;

namespace Game.Item
{
    public class ItemAudioHelper<TItem> where TItem : ItemBase<TItem>
    {
        public ItemAudioHelper(TItem owner,ItemAudioDatas itemAudioDatas)
        {
            this.owner = owner;
            this.audioDatas = itemAudioDatas;
            this.audioSource = new AudioSource();
            var itemAudioObj = new GameObject("Audio Item Obj");
            this.audioSource = itemAudioObj.AddComponent<AudioSource>();
            Debug.Log("èâä˙âªäÆóπÇæÇÊ");
        }

        TItem owner;
        AudioSource audioSource;
        ItemAudioDatas audioDatas;
        public void PlayPickUpAudio()
        {
            Debug.Log("âπÇ™ñ¬ÇËÇ‹Ç∑");
            audioSource.PlayOneShot(audioDatas.PickUpAudio);
        }
    }
}

