using UnityEngine;
using JetBrains.Annotations;
using UnityEditor.Animations;
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

    public static float GetStateSpeed(this Animator animator,string targetClipName)
    {
        var runTimeAnimatorController = animator.runtimeAnimatorController;
        var overrideController = runTimeAnimatorController as AnimatorOverrideController;
        if (runTimeAnimatorController == null && overrideController == null) throw new System.Exception("Animator is not found!!");
        //overrideされたアニメーターの場合、AnimatorOverrideControllerのrunTimeAnimatorControllerからAnimatorControllerを取得しないといけない
        AnimatorController animatorController = overrideController != null
                                                ? overrideController.runtimeAnimatorController as AnimatorController
                                                : runTimeAnimatorController as AnimatorController;
        foreach (var layer in animatorController.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                var motion = state.state.motion;
                if (motion == null) continue;
                if(motion.name == targetClipName)
                {
                    return state.state.speed;
                }
            }
        }

        return default;
    }
}
