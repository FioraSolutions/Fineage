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
            if (named != null && SameLocomotionTake(named.name, clipName))
                return named;

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
            overrideController.GetOverrides(overrides);
            for (int i = 0; i < overrides.Count; i++)
            {
                AnimationClip key = overrides[i].Key;
                if (key == null || !SameLocomotionTake(key.name, clipName))
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
            if (clip != null && SameLocomotionTake(clip.name, clipName))
                return clip;
        }

        return null;
    }

    /// <summary>
    /// Baked creature takes keep the mesh stem and the Interlude sequence:
    /// <c>dryad_m00.ao_Wait</c>, <c>giant_spider_m00.ao_Walk.001</c>.
    /// The locomotion slot is the sequence token (<c>wait</c>, <c>walk</c>, <c>run</c>),
    /// case-insensitive, after <c>.ao_</c> and a Unity duplicate suffix.
    /// A weapon take such as <c>walk_1hs</c> stays a different slot.
    /// </summary>
    public static bool SameLocomotionTake(string clipName, string wanted)
    {
        if (string.IsNullOrEmpty(clipName) || string.IsNullOrEmpty(wanted))
            return false;
        if (clipName.Equals(wanted, StringComparison.OrdinalIgnoreCase))
            return true;
        string token = TakeToken(clipName);
        return !token.Equals(clipName, StringComparison.OrdinalIgnoreCase)
            && token.Equals(wanted, StringComparison.OrdinalIgnoreCase);
    }

    public static string TakeToken(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return clipName;
        string token = clipName;
        int ao = token.LastIndexOf(".ao_", StringComparison.OrdinalIgnoreCase);
        if (ao >= 0 && ao + 4 < token.Length)
            token = token.Substring(ao + 4);
        int dot = token.LastIndexOf('.');
        if (dot > 0 && DigitsOnly(token, dot + 1))
            token = token.Substring(0, dot);
        return token;
    }

    static bool DigitsOnly(string text, int start)
    {
        if (start >= text.Length)
            return false;
        for (int i = start; i < text.Length; i++)
        {
            if (text[i] < '0' || text[i] > '9')
                return false;
        }
        return true;
    }
}