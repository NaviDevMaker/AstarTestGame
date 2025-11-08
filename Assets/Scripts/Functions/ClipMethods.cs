using UnityEngine;

public static class ClipMethods
{
    public static float GetLength(this AnimationClip clip) => clip.length;

    public static float GetControllerLength(this Animator animator,string targetClipName)
    {
        var runTimeController = animator.runtimeAnimatorController;
        var clips = runTimeController.animationClips;

        foreach (var clip in clips)
        {
            if (clip.name == targetClipName) return clip.length;
        }

        return default;
    }
}
