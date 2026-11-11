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

        string key = overrideAnimName + "_" + raceName;
        if (cache.TryGetValue(key, out SkillClips clips))
        {
            clips.Load();
            return clips.GetClip();
        }
        return null;
    }
}
