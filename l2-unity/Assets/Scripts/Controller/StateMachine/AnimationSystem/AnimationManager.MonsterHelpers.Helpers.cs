using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public partial class AnimationManager
{
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
