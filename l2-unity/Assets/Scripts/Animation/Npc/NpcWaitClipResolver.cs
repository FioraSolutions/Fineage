using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Resolve Interlude NPC base wait clip from <see cref="Npc_Basic"/> override slots.</summary>
public static class NpcWaitClipResolver
{
    public static AnimationClip ResolveBaseWait(Animator animator)
    {
        if (animator == null)
            return null;

        if (animator.runtimeAnimatorController is AnimatorOverrideController ov)
            return FindWaitInOverrides(ov);
        return null;
    }

    static AnimationClip FindWaitInOverrides(AnimatorOverrideController ov)
    {
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        ov.GetOverrides(pairs);
        for (int i = 0; i < pairs.Count; i++)
        {
            AnimationClip slot = pairs[i].Key;
            if (slot == null || !IsBaseWaitSlot(slot.name))
                continue;
            AnimationClip clip = pairs[i].Value ?? slot;
            if (clip != null)
                return clip;
        }

        return null;
    }

    /// <summary>
    /// The <c>wait</c> state of Npc_Basic.controller is bound to the template clip
    /// <c>a_traderA_FHuman_m00.ao_wait_Hand.004</c> (unarmed take), so the slot's take token is
    /// <c>wait_Hand</c>, never plain <c>wait</c>. Matching only <c>wait</c> reported every human NPC
    /// override ("missing base wait animation") although its wait slot is overridden.
    /// </summary>
    public static bool IsBaseWaitSlot(string slotName)
    {
        if (LocomotionGait.SameLocomotionTake(slotName, "wait"))
            return true;
        return LocomotionGait.SameLocomotionTake(slotName, "wait_Hand");
    }

    public static bool IsBaseLocomotionLoopName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return false;
        string n = clipName.ToLowerInvariant();
        if (n.Contains("spwait") || n.Contains("social") || n.Contains("atk01"))
            return false;
        if (n.Contains("death") && !n.Contains("deathwait") && !n.Contains("atkwait"))
            return false;
        return n.Contains("wait") || n.Contains("walk") || n.Contains("run");
    }
}