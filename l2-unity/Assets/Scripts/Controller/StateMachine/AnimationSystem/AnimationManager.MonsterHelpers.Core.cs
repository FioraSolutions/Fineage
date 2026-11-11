using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public partial class AnimationManager
{
    static float ResolveAttackClipLength(Animator animator, string stateName)
    {
        if (animator == null)
            return 1f;

        if (animator.IsInTransition(0))
        {
            AnimatorClipInfo[] next = animator.GetNextAnimatorClipInfo(0);
            if (next != null && next.Length > 0 && next[0].clip != null)
                return next[0].clip.length;
        }

        if (TryFindNamedClipLength(animator, stateName, out float namedLength))
            return namedLength;

        return 1f;
    }

    static bool TryFindNamedClipLength(Animator animator, string nameContains, out float length)
    {
        length = 0f;
        if (animator == null || string.IsNullOrEmpty(nameContains))
            return false;

        RuntimeAnimatorController rac = animator.runtimeAnimatorController;
        if (rac != null)
        {
            AnimationClip[] clips = rac.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip clip = clips[i];
                if (clip == null)
                    continue;
                if (clip.name == nameContains ||
                    clip.name.IndexOf(nameContains, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    length = clip.length;
                    return true;
                }
            }
        }

        float cached = AnimationDataCache.GetOverrideLength(animator, nameContains);
        if (cached > 0.01f)
        {
            length = cached;
            return true;
        }

        return false;
    }

    float GetClipLengthByName(int objectId, string nameContains, float fallback)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return fallback;

        if (!TryFindNamedClipLength(controller.GetAnimator(), nameContains, out float length))
            return fallback;
        return length;
    }
}
