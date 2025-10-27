using UnityEngine;

[CreateAssetMenu]
public class AnimatorHash : ScriptableObject
{
    [SerializeField] string walkClipName;
    [SerializeField] string attackClipName;
    [SerializeField] string deathClipName;

    public int walkHash => Animator.StringToHash(walkClipName);

    public int attackHash => Animator.StringToHash(attackClipName);
    public int deathHash => Animator.StringToHash(deathClipName);
}
