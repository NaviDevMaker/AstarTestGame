using UnityEngine;
using Cysharp.Threading.Tasks;
[CreateAssetMenu]
public class AnimationData : ScriptableObject
{
    [Header("Hash Names")]
    [SerializeField] string walkHashName;
    [SerializeField] string attackHashName;
    [SerializeField] string pickUpHashName;
    [SerializeField] string deathHashName;

    [Header("Clip Names")]
    [SerializeField] string walkClipName;
    [SerializeField] string attackClipName;
    [SerializeField] string pickUpClipName;
    [SerializeField] string deathClipName;

    [Header("Layers")]
    [SerializeField] int baseLayerIndex;
    [SerializeField] int attackLayerIndex;
    [SerializeField] int pickUpLayerIndex;

    [Header("")]
    public int WalkHash => Animator.StringToHash(walkHashName);

    public int AttackHash => Animator.StringToHash(attackHashName);
    public int PickUpHash => Animator.StringToHash(pickUpHashName);
    public int DeathHash => Animator.StringToHash(deathHashName);

    public string WalkClipName => walkClipName;
    public string AttackClipName  => attackClipName;
    public string PickUpClipName => pickUpClipName;
    public string DeathClipName => deathClipName;

    public int BaseLayerIndex  => baseLayerIndex;
    public int AttackLayerIndex  => attackLayerIndex;
    public int PickUpLayerIndex => pickUpLayerIndex;

    public async UniTask<AnimationClip> LoadClip(string clipName)
    {
        var address = $"Animations/Homeless/{clipName}";
        return await GetAssetsMethods.GetAsset<AnimationClip>(address);
    }
}
