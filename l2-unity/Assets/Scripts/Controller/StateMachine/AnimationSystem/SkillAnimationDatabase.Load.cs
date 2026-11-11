using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    public static void Initialize()
    {
        if (_raceCacheMMagic.Count > 0)
            return;

        //MMagic Эти ролики берет для переиспользования (этот override аниматор)
        _raceCacheMMagic.Add("CastLong_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastLong_MMagic"));
        _raceCacheMMagic.Add("CastShort_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastShort_MMagic"));
        _raceCacheMMagic.Add("CastMid_"+ MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastMid_MMagic"));
        _raceCacheMMagic.Add("CastEnd_"+ MMagic, new SkillClips (FOLDER_MMagic, "MMagic_M000_b.ao_CastEnd_MMagic"));
        _raceCacheMMagic.Add("MagicShot_" + MMagic, new SkillClips (FOLDER_MMagic, "MMagic_M000_b.ao_MagicShot_MMagic"));
        _raceCacheMMagic.Add("MagicNoTarget_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicNoTarget_MMagic"));
        _raceCacheMMagic.Add("MagicNotarget_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicNoTarget_MMagic"));
        _raceCacheMMagic.Add("MagicThrow_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicThrow_MMagic"));

        // FMagic.overrideController: Cast* body clips are not imported yet — reuse MMagic cast
        // poses so female mage still plays CastMid/End/Shot after MagicSkillUse (plan #mesh FX).
        _raceCacheFMagic.Add("CastLong_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastLong_MMagic"));
        _raceCacheFMagic.Add("CastShort_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastShort_MMagic"));
        _raceCacheFMagic.Add("CastMid_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastMid_MMagic"));
        _raceCacheFMagic.Add("CastEnd_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastEnd_MMagic"));
        _raceCacheFMagic.Add("MagicShot_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicShot_MMagic"));
        _raceCacheFMagic.Add("MagicThrow_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicThrow_MMagic"));
        _raceCacheFMagic.Add("MagicNoTarget_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicNoTarget_FMagic"));
        _raceCacheFMagic.Add("MagicNotarget_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicNoTarget_FMagic"));

        _raceCacheMFighter.Add("CastShort_" + MFighter, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastShort_MMagic"));
        _raceCacheMFighter.Add("CastMid_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_CastMid_MFighter"));
        _raceCacheMFighter.Add("CastEnd_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_CastEnd_MFighter"));
        _raceCacheMFighter.Add("MagicShot_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_MagicShot_MFighter"));
        _raceCacheMFighter.Add("MagicThrow_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_MagicThrow_MFighter"));

        // MElf (light elf male) — clips from Elf.ukx via L2MElfAnimImporter, same keys as MMagic
        _raceCacheMElf.Add("CastLong_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastLong_MElf"));
        _raceCacheMElf.Add("CastShort_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastShort_MElf"));
        _raceCacheMElf.Add("CastMid_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastMid_MElf"));
        _raceCacheMElf.Add("CastEnd_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastEnd_MElf"));
        _raceCacheMElf.Add("MagicShot_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicShot_MElf"));
        _raceCacheMElf.Add("MagicNoTarget_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicNotarget_MElf"));
        _raceCacheMElf.Add("MagicNotarget_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicNotarget_MElf"));
        _raceCacheMElf.Add("MagicThrow_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicThrow_MElf"));

        //эти ролики берет для замены на свои (т.е это оригинальный аниматор)
        //FDarkElf
        _raceCacheFDarkElf.Add("CastLong_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastLong_FDarkElf"));
        _raceCacheFDarkElf.Add("CastShort_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastShort_FDarkElf"));
        _raceCacheFDarkElf.Add("CastMid_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastMid_FDarkElf"));
        _raceCacheFDarkElf.Add("CastEnd_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastEnd_FDarkElf"));
        _raceCacheFDarkElf.Add("MagicShot_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_MagicShot_A_FDarkElf"));
        _raceCacheFDarkElf.Add("MagicThrow_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_MagicShot_A_FDarkElf"));


    }

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
