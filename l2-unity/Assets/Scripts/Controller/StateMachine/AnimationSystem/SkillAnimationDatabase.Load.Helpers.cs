using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    public static void LoadRaceAnimations(string raceName)
    {
        Dictionary<string, SkillClips> cache = CacheFor(raceName);
        if (cache == null)
            return;

        foreach (var item in cache)
            item.Value.Load();
    }

    public static AnimationClip GetOverrideClip(string overrideAnimName, string raceName)
    {
        if (string.IsNullOrEmpty(overrideAnimName))
            return null;

        Dictionary<string, SkillClips> cache = CacheFor(raceName);
        AnimationClip clip = cache != null ? LoadCachedClip(cache, overrideAnimName, raceName) : null;
        if (clip != null)
            return clip;

        clip = TryLoadRaceOverrideClip(overrideAnimName, raceName);
        if (clip != null)
            return clip;

        if (cache != null)
        {
            if (string.Equals(overrideAnimName, "MagicNoTarget", StringComparison.Ordinal))
                clip = LoadCachedClip(cache, "MagicNotarget", raceName);
            else if (string.Equals(overrideAnimName, "MagicNotarget", StringComparison.Ordinal))
                clip = LoadCachedClip(cache, "MagicNoTarget", raceName);
            if (clip != null)
                return clip;

            string fallback = ResolveOverrideCompletionEventName(overrideAnimName);
            if (!string.IsNullOrEmpty(fallback) && !string.Equals(fallback, overrideAnimName, StringComparison.Ordinal))
            {
                clip = LoadCachedClip(cache, fallback, raceName);
                if (clip == null)
                    clip = TryLoadRaceOverrideClip(fallback, raceName);
                if (clip != null)
                {
                    SkillAnimationResolver.LogOverrideCompletionFallback(
                        overrideAnimName, fallback, raceName, clip.name);
                    return clip;
                }
            }
        }

        return null;
    }

    static AnimationClip LoadCachedClip(Dictionary<string, SkillClips> cache, string animName, string raceName)
    {
        string key = animName + "_" + raceName;
        if (!cache.TryGetValue(key, out SkillClips clips) || clips == null)
            return null;
        clips.Load();
        return clips.GetClip();
    }
}
