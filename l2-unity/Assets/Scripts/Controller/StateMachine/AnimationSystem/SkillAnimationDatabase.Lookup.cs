using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    static Dictionary<string, SkillClips> CacheFor(string raceName)
    {
        Initialize();
        if (string.IsNullOrEmpty(raceName))
            return null;
        // Exact match first; then contains — overrideController names are usually "MMagic"
        // but some prefabs report a longer controller name.
        if (raceName == MMagic || ContainsRaceToken(raceName, MMagic))
            return _raceCacheMMagic;
        if (raceName == FMagic || ContainsRaceToken(raceName, FMagic))
            return _raceCacheFMagic;
        if (raceName == MFighter || ContainsRaceToken(raceName, MFighter))
            return _raceCacheMFighter;
        if (raceName == MElf || ContainsRaceToken(raceName, MElf))
            return _raceCacheMElf;
        return null;
    }
}
