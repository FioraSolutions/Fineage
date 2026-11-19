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
        // Longer / more specific tokens first so FDarkElf is not misread as FElf, etc.
        if (raceName == FDarkElf || ContainsRaceToken(raceName, FDarkElf))
            return _raceCacheFDarkElf;
        if (raceName == MDarkElf || ContainsRaceToken(raceName, MDarkElf))
            return _raceCacheMDarkElf;
        if (raceName == FShaman || ContainsRaceToken(raceName, FShaman))
            return _raceCacheFShaman;
        if (raceName == MShaman || ContainsRaceToken(raceName, MShaman))
            return _raceCacheMShaman;
        if (raceName == FFighter || ContainsRaceToken(raceName, FFighter))
            return _raceCacheFFighter;
        if (raceName == MFighter || ContainsRaceToken(raceName, MFighter))
            return _raceCacheMFighter;
        if (raceName == FMagic || ContainsRaceToken(raceName, FMagic))
            return _raceCacheFMagic;
        if (raceName == MMagic || ContainsRaceToken(raceName, MMagic))
            return _raceCacheMMagic;
        if (raceName == FElf || ContainsRaceToken(raceName, FElf))
            return _raceCacheFElf;
        if (raceName == MElf || ContainsRaceToken(raceName, MElf))
            return _raceCacheMElf;
        if (raceName == FDwarf || ContainsRaceToken(raceName, FDwarf))
            return _raceCacheFDwarf;
        if (raceName == MDwarf || ContainsRaceToken(raceName, MDwarf))
            return _raceCacheMDwarf;
        if (raceName == FOrc || ContainsRaceToken(raceName, FOrc))
            return _raceCacheFOrc;
        if (raceName == MOrc || ContainsRaceToken(raceName, MOrc))
            return _raceCacheMOrc;
        return null;
    }
}
