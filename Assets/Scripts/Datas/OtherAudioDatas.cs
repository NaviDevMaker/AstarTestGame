using UnityEngine;

[CreateAssetMenu]
public class OtherAudioDatas : ScriptableObject
{
    [Header("Main Audios")]
    [SerializeField] AudioClip mainAudio;

    [Header("SE Audios")]
    [SerializeField] AudioClip countDownClip;
    [SerializeField] AudioClip recordAppearClip;
    [SerializeField] AudioClip battleClip;
    public AudioClip CountDownClip => countDownClip;
    public AudioClip MainAudio => mainAudio;
    public AudioClip RecordAppearClip=> recordAppearClip;
    public AudioClip BattleClip => battleClip;
}
