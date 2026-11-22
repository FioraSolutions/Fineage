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

        _raceCacheFMagic.Add("CastLong_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_castLong_FMagic"));
        _raceCacheFMagic.Add("CastShort_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_CastShort_FMagic"));
        _raceCacheFMagic.Add("CastMid_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_castMid_FMagic"));
        _raceCacheFMagic.Add("CastEnd_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_castEnd_FMagic"));
        _raceCacheFMagic.Add("MagicShot_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicShot_FMagic"));
        _raceCacheFMagic.Add("MagicNoTarget_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicNoTarget_FMagic"));
        _raceCacheFMagic.Add("MagicNotarget_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicNoTarget_FMagic"));
        _raceCacheFMagic.Add("MagicThrow_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicThrow_FMagic"));

        _raceCacheFFighter.Add("CastLong_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_CastLong_FFighter"));
        _raceCacheFFighter.Add("CastShort_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_CastShort_FFighter"));
        _raceCacheFFighter.Add("CastMid_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_CastMid_FFighter"));
        _raceCacheFFighter.Add("CastEnd_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_CastEnd_FFighter"));
        _raceCacheFFighter.Add("MagicShot_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_MagicShot_FFighter"));
        _raceCacheFFighter.Add("MagicNoTarget_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_MagicNotarget_FFighter"));
        _raceCacheFFighter.Add("MagicNotarget_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_MagicNotarget_FFighter"));
        _raceCacheFFighter.Add("MagicThrow_" + FFighter, new SkillClips(FOLDER_FFighter, "FFighter_m000_b.ao_MagicThrow_FFighter"));

        // MFighter never exported CastLong/CastShort/MagicNoTarget. GetOverrideClip falls
        // those slots back to CastMid / MagicShot so a 5s tag-i channel still plays motion.
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
        _raceCacheFDarkElf.Add("MagicShot_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m000_b.ao_MagicShot_FDarkElf"));
        _raceCacheFDarkElf.Add("MagicNoTarget_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m000_b.ao_MagicNotarget_FDarkElf"));
        _raceCacheFDarkElf.Add("MagicNotarget_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m000_b.ao_MagicNotarget_FDarkElf"));
        _raceCacheFDarkElf.Add("MagicThrow_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m000_b.ao_MagicThrow_FDarkElf"));

        AddStandardCastCache(_raceCacheFElf, FElf, FOLDER_FElf, "FElf_m000_b", "MagicNotarget");
        AddStandardCastCache(_raceCacheMDarkElf, MDarkElf, FOLDER_MDarkElf, "MDarkElf_m000_b", "MagicNotarget");
        AddStandardCastCache(_raceCacheMDwarf, MDwarf, FOLDER_MDwarf, "MDwarf_m000_b", "MagicNotarget");
        AddStandardCastCache(_raceCacheFDwarf, FDwarf, FOLDER_FDwarf, "FDwarf_m001_b", "MagicNotarget");
        AddStandardCastCache(_raceCacheMOrc, MOrc, FOLDER_MOrc, "MOrc_m000_b", "MagicNotarget");
        AddStandardCastCache(_raceCacheFOrc, FOrc, FOLDER_FOrc, "FOrc_m000_b", "MagicNotarget");
        AddStandardCastCache(_raceCacheMShaman, MShaman, FOLDER_MShaman, "MShaman_m000_b", "MagicNotarget");
        AddStandardCastCache(_raceCacheFShaman, FShaman, FOLDER_FShaman, "FShaman_m000_b", "MagicNotarget");
    }

    static void AddStandardCastCache(
        Dictionary<string, SkillClips> cache,
        string race,
        string folder,
        string meshStem,
        string noTargetClip)
    {
        cache.Add("CastLong_" + race, new SkillClips(folder, meshStem + ".ao_CastLong_" + race));
        cache.Add("CastShort_" + race, new SkillClips(folder, meshStem + ".ao_CastShort_" + race));
        cache.Add("CastMid_" + race, new SkillClips(folder, meshStem + ".ao_CastMid_" + race));
        cache.Add("CastEnd_" + race, new SkillClips(folder, meshStem + ".ao_CastEnd_" + race));
        cache.Add("MagicShot_" + race, new SkillClips(folder, meshStem + ".ao_MagicShot_" + race));
        cache.Add("MagicThrow_" + race, new SkillClips(folder, meshStem + ".ao_MagicThrow_" + race));
        string noTargetName = meshStem + ".ao_" + noTargetClip + "_" + race;
        cache.Add("MagicNoTarget_" + race, new SkillClips(folder, noTargetName));
        cache.Add("MagicNotarget_" + race, new SkillClips(folder, noTargetName));
    }
}
