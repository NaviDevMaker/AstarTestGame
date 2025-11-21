using UnityEngine;

[CreateAssetMenu]
public class PlayerTweenFieldDatas : ScriptableObject
{
    [Header("Blink tween fields")]
    [SerializeField] float duration;
    [SerializeField] float targetAlpha;
    public float blinkDuration  => duration;
    public float TargetAlpha  => targetAlpha;
}
