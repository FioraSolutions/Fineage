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
        Dictionary<string, SkillClips> cache = CacheFor(raceName);
        if (cache == null || string.IsNullOrEmpty(overrideAnimName))
            return null;

        AnimationClip clip = LoadCachedClip(cache, overrideAnimName, raceName);
        if (clip != null)
            return clip;

        // Official packs spell MagicNoTarget as either casing; try the other before giving up.
        if (string.Equals(overrideAnimName, "MagicNoTarget", StringComparison.Ordinal))
            clip = LoadCachedClip(cache, "MagicNotarget", raceName);
        else if (string.Equals(overrideAnimName, "MagicNotarget", StringComparison.Ordinal))
            clip = LoadCachedClip(cache, "MagicNoTarget", raceName);
        if (clip != null)
            return clip;

        // CastLong/CastShort share the mid slot on meshes that never exported a long/short clip
        // (MFighter has CastMid/CastEnd/MagicThrow only). MagicNoTarget/MagicThrow fall back to
        // MagicShot — same mapping already used for completion events.
        string fallback = ResolveOverrideCompletionEventName(overrideAnimName);
        if (!string.IsNullOrEmpty(fallback) && !string.Equals(fallback, overrideAnimName, StringComparison.Ordinal))
            return LoadCachedClip(cache, fallback, raceName);

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
