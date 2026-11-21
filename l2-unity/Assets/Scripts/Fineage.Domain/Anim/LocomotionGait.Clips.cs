using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class LocomotionGait
{
    public const string LogTag = "[LocomotionGait]";

    static readonly HashSet<string> LoggedMissingClips = new HashSet<string>();

    public static void ClearMissingClipLogForTests() => LoggedMissingClips.Clear();

    public static void LogMissingLocomotionClip(
        Animator animator,
        string clipName,
        string profile = null)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        string animatorName = animator != null ? animator.name : "null";
        string profileKey = string.IsNullOrEmpty(profile) ? string.Empty : profile;
        string key = animatorName + "@" + clipName + "@" + profileKey;
        if (!LoggedMissingClips.Add(key))
            return;

        string profileSuffix = string.IsNullOrEmpty(profile)
            ? string.Empty
            : " profile=" + profile;
        Debug.LogError(
            LogTag + " missing locomotion clip=\"" + clipName + "\" animator="
            + animatorName + profileSuffix
            + " (run_speed/walk_speed may stall at 0)");
    }

    public static AnimationClip FindClip(Animator animator, string clipName)
    {
        if (animator == null || string.IsNullOrEmpty(clipName))
            return null;

        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        if (controller == null)
            return null;

        var overrideController = controller as AnimatorOverrideController;
        if (overrideController != null)
        {
            AnimationClip named = overrideController[clipName];
            if (named != null && ClipNameMatches(named.name, clipName))
                return named;

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
            overrideController.GetOverrides(overrides);
            for (int i = 0; i < overrides.Count; i++)
            {
                AnimationClip key = overrides[i].Key;
                if (key == null || !ClipNameMatches(key.name, clipName))
                    continue;
                AnimationClip value = overrides[i].Value;
                return value != null ? value : key;
            }
        }

        AnimationClip[] clips = controller.animationClips;
        if (clips == null)
            return null;
        for (int i = 0; i < clips.Length; i++)
        {
            AnimationClip clip = clips[i];
            if (clip != null && ClipNameMatches(clip.name, clipName))
                return clip;
        }

        return null;
    }

    static bool ClipNameMatches(string clipName, string wanted)
    {
        if (string.IsNullOrEmpty(clipName) || string.IsNullOrEmpty(wanted))
            return false;
        return clipName.Equals(wanted, StringComparison.OrdinalIgnoreCase);
    }
}
