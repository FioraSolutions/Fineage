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
            if (slot == null || !LocomotionGait.SameLocomotionTake(slot.name, "wait"))
                continue;
            AnimationClip clip = pairs[i].Value ?? slot;
            if (clip != null)
                return clip;
        }

        return null;
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