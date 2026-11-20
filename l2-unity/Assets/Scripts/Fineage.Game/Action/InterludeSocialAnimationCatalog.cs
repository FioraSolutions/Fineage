using System;
using System.Collections.Generic;

/// <summary>
/// Maps SocialAction / RequestActionUse social ids → PSA clip take stem per race (shared table, no per-race ifs in callers).
/// Clip files live under <c>Assets/Resources/Data/Animations/&lt;family&gt;/&lt;Race&gt;/Clips/</c>.
/// SocialAction id equals RequestActionUse id (Fineage-Backend fan-out 1:1).
/// </summary>
public static class InterludeSocialAnimationCatalog
{
    /// <summary>PSA take suffix (e.g. Social_nod). Animator plays via social01 override.</summary>
    public static bool TryGetClipStem(int socialActionId, out string clipStem)
    {
        clipStem = null;
        switch (socialActionId)
        {
            case 12: clipStem = "Social_nod"; return true;
            case 13: clipStem = "Social_victory"; return true;
            case 14: clipStem = "Social_Atk"; return true;
            case 24: clipStem = "Social_yes"; return true;
            case 25: clipStem = "Social_no"; return true;
            case 26: clipStem = "Social_bow"; return true;
            case 29: clipStem = "Social_unaware"; return true;
            case 30: clipStem = "Social_waiting_a"; return true;
            case 31: clipStem = "Social_laugh"; return true;
            case 33: clipStem = "Social_clap"; return true;
            case 34: clipStem = "Social_dance"; return true;
            case 35: clipStem = "Social_sad"; return true;
            case 62: clipStem = "Social_SpWait01"; return true;
            case 66: clipStem = "Social_SpWait02"; return true;
            default: return false;
        }
    }

    static readonly (CharacterRaceAnimation Race, string FamilyFolder, string ClipRaceSuffix)[] RaceLayout =
    {
        (CharacterRaceAnimation.MFighter, "Fighter", "MFighter"),
        (CharacterRaceAnimation.FFighter, "Fighter", "FFighter"),
        (CharacterRaceAnimation.MElf, "Elf", "MElf"),
        (CharacterRaceAnimation.FElf, "Elf", "FElf"),
        (CharacterRaceAnimation.MDarkElf, "DarkElf", "MDarkElf"),
        (CharacterRaceAnimation.FDarkElf, "DarkElf", "FDarkElf"),
        (CharacterRaceAnimation.MDwarf, "Dwarf", "MDwarf"),
        (CharacterRaceAnimation.FDwarf, "Dwarf", "FDwarf"),
        (CharacterRaceAnimation.MOrc, "Orc", "MOrc"),
        (CharacterRaceAnimation.FOrc, "Orc", "FOrc"),
        (CharacterRaceAnimation.MShaman, "Orc", "MShaman"),
        (CharacterRaceAnimation.FShaman, "Orc", "FShaman"),
        (CharacterRaceAnimation.MMagic, "Magic", "MMagic"),
        (CharacterRaceAnimation.FMagic, "Magic", "FMagic"),
    };

    public static IEnumerable<(CharacterRaceAnimation Race, string FamilyFolder, string ClipRaceSuffix)> AllRaces()
    {
        for (int i = 0; i < RaceLayout.Length; i++)
            yield return RaceLayout[i];
    }

    /// <summary>Resources folder prefix without clip file name (Clips directory).</summary>
    public static string ClipResourcesFolder(CharacterRaceAnimation race)
    {
        for (int i = 0; i < RaceLayout.Length; i++)
        {
            if (RaceLayout[i].Race == race)
                return "Data/Animations/" + RaceLayout[i].FamilyFolder + "/" + RaceLayout[i].ClipRaceSuffix + "/Clips";
        }

        return null;
    }

    /// <summary>
    /// Expected clip asset file name fragment for logging/tests (matches imported .anim naming).
    /// </summary>
    public static string ExpectedClipFileSubstring(CharacterRaceAnimation race, string clipStem)
    {
        for (int i = 0; i < RaceLayout.Length; i++)
        {
            if (RaceLayout[i].Race != race)
                continue;
            return ".ao_" + clipStem + "_" + RaceLayout[i].ClipRaceSuffix;
        }

        return ".ao_" + clipStem;
    }

    public static bool TryLoadClip(CharacterRaceAnimation race, string clipStem, out UnityEngine.AnimationClip clip)
    {
        clip = null;
        string folder = ClipResourcesFolder(race);
        if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(clipStem))
            return false;

        AnimationClip[] clips = UnityEngine.Resources.LoadAll<AnimationClip>(folder);
        if (clips == null || clips.Length == 0)
            return false;

        string needle = ".ao_" + clipStem + "_";
        for (int i = 0; i < clips.Length; i++)
        {
            AnimationClip c = clips[i];
            if (c != null && c.name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                clip = c;
                return true;
            }
        }

        // FDwarf uses m001_b mesh prefix in some imports.
        needle = ".ao_" + clipStem;
        for (int i = 0; i < clips.Length; i++)
        {
            AnimationClip c = clips[i];
            if (c != null && c.name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                clip = c;
                return true;
            }
        }

        return false;
    }

    public static string GenderLabel(CharacterRaceAnimation race)
    {
        switch (race)
        {
            case CharacterRaceAnimation.FElf:
            case CharacterRaceAnimation.FDarkElf:
            case CharacterRaceAnimation.FDwarf:
            case CharacterRaceAnimation.FOrc:
            case CharacterRaceAnimation.FShaman:
            case CharacterRaceAnimation.FMagic:
            case CharacterRaceAnimation.FFighter:
                return "female";
            default:
                return "male";
        }
    }
}