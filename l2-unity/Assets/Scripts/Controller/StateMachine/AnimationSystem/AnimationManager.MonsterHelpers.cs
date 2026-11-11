using System;
using System.Collections.Generic;
using UnityEngine;

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

    public float PlayExactAnimatorState(int objectId, string stateName, bool snapToEnd = false)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null || string.IsNullOrEmpty(stateName))
            return 1.6f;

        AnimationEventForwarder.BindAnimator(controller);
        controller.SetEnabled(true);
        DesableLastPlayerAnimationElseTrue(objectId, controller);
        SetRecentName(objectId, stateName);

        bool isPickup = string.Equals(stateName, "pickup", System.StringComparison.OrdinalIgnoreCase);
        // Override clip is PicItem_*, not "pickup" — resolve by PicItem for length.
        float clipLength = isPickup
            ? GetClipLengthByName(objectId, "PicItem", AnimPlayback.PickupFallbackSeconds)
            : GetClipLengthByName(objectId, stateName, 1.6f);

        // PicItem: authored timing only (Interlude ~0.17–0.25s). Not bound to server clocks.
        AnimPlayback.PrepareGesture(controller);

        Animator animator = controller.GetAnimator();
        if (snapToEnd && animator != null)
        {
            animator.Play(stateName, 0, 1f);
            animator.Update(0f);
        }
        else
        {
            controller.CrossFadeInFixedTime(stateName, LocomotionCrossFadeSettings.FixedDuration);
        }

        if (isPickup)
            return AnimPlayback.PickupPlaySeconds(clipLength);
        return Mathf.Max(0.4f, clipLength);
    }

    public float PlayGesture(int objectId, string stateName)
    {
        return PlayExactAnimatorState(objectId, stateName, snapToEnd: false);
    }

    public float[] GetOverrideClipsDurations(int objectId, string[]cycle)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return new float[cycle != null ? cycle.Length : 0];

        float[] durations = new float[cycle.Length];

        using (MagicSkillUseTrace.Measure("anim.clipLookup"))
        {
            for(int i=0; i< cycle.Length; i++)
            {
                AnimationClip clip = SkillAnimationDatabase.GetOverrideClip(cycle[i], controller.GetAnimatorName());
                if(clip != null)
                {
                    durations[i] = clip.length;
                }
            }
        }

        return durations;
    }

    public float GetOverrideEventTimeByName(int objectId, string[] cycle, string eventName)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return 0;

        for (int i = 0; i < cycle.Length; i++)
        {
            AnimationClip clip = SkillAnimationDatabase.GetOverrideClip(cycle[i], controller.GetAnimatorName());
            if (clip != null)
            {
                float timeStartEvent = controller.GetEventTimeByName(clip, eventName);
                if (timeStartEvent != 0) return timeStartEvent;
            }
        }
        return 0;
    }
}
