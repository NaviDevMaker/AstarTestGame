using UnityEngine;

[CreateAssetMenu]
public class AnimationData : ScriptableObject
{
    [Header("Hash Names")]
    [SerializeField] string walkHashName;
    [SerializeField] string attackHashName;
    [SerializeField] string deathHashName;

    [Header("Clip Names")]
    [SerializeField] string walkClipName;
    [SerializeField] string attackClipName;
    [SerializeField] string deathClipName;

    [Header("Layers")]
    [SerializeField] int baseLayerIndex;
    [SerializeField] int attackLayerIndex;
    public int WalkHash => Animator.StringToHash(walkHashName);

    public int AttackHash => Animator.StringToHash(attackHashName);
    public int DeathHash => Animator.StringToHash(deathHashName);

    public string WalkClipName => walkClipName;
    public string AttackClipName  => attackClipName;
    public string DeathClipName => deathClipName;

    public int BaseLayerIndex { get => baseLayerIndex; set => baseLayerIndex = value; }
    public int AttackLayerIndex { get => attackLayerIndex; set => attackLayerIndex = value; }
}
