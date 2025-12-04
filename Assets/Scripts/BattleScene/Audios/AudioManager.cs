using System;
using UnityEngine;

public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    [SerializeField] AudioSource mainAudioSource;

    [SerializeField] OtherAudioDatas otherAudioDatas;
    [SerializeField] SESetting sESetting;

    [Serializable]
    class SESetting
    {
        [SerializeField] AudioSource sEAudioSource;
        [SerializeField] float originalVolume;
        [SerializeField] float originalPitch;

        public AudioSource SEAudioSource => sEAudioSource;
        public float OriginalVolume  => originalVolume;
        public float OriginalPitch => originalPitch;
    }

    public void Initialize()
    {
        sESetting.SEAudioSource.volume = sESetting.OriginalVolume;
        PlayMainAudio();
    }
    void PlayMainAudio()
    {
        mainAudioSource.clip = otherAudioDatas.MainAudio;
        mainAudioSource.loop = true;
        mainAudioSource.volume = 1.0f;
        mainAudioSource.Play();
    }
    void PlaySE(AudioSource targetSource,AudioClip audioClip) => targetSource.PlayOneShot(audioClip);
    public void PlayCountDownSE(bool isLastCount)
    {
        var temp = new GameObject("TempAudioSource");
        var tempAudioSource = temp.AddComponent<AudioSource>();
        var originalPicth = sESetting.OriginalPitch;
        if (isLastCount) tempAudioSource.pitch = isLastCount ? originalPicth / 2
                                                 : originalPicth;
        var clipLength = otherAudioDatas.CountDownClip.length / tempAudioSource.pitch;
        PlaySE(tempAudioSource,otherAudioDatas.CountDownClip);
        Destroy(tempAudioSource, clipLength);
    }
    public void PlayRecordSE()
    {
        sESetting.SEAudioSource.pitch = sESetting.OriginalPitch / 2;
        PlaySE(sESetting.SEAudioSource, otherAudioDatas.RecordAppearClip);
    }
    public void PlayBattleSE() => PlaySE(sESetting.SEAudioSource,otherAudioDatas.BattleClip);
}
