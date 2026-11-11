using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    static void PlayVoiceKind(Entity caster, string kind)
    {
        string prefix = VoicePrefixFor(caster);
        if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(kind))
            return;
        string primary = prefix + "_" + kind;
        if (PlayVoiceClip(primary))
        {
            Debug.Log("[CastSync] VOICE play=" + primary);
            return;
        }
        // Soft fallbacks only within the same phase family.
        if (kind == "Notarget" && PlayVoiceClip(prefix + "_Shot"))
        {
            Debug.Log("[CastSync] VOICE play=" + prefix + "_Shot (fallback from Notarget)");
            return;
        }
        if (kind == "Shot" && PlayVoiceClip(prefix + "_Throw"))
        {
            Debug.Log("[CastSync] VOICE play=" + prefix + "_Throw (fallback from Shot)");
            return;
        }
        Debug.LogWarning("[CastSync] VOICE miss kind=" + kind + " tried=" + primary);
    }

    static string KindForRace(string[] kinds, Entity caster)
    {
        if (kinds == null || kinds.Length == 0 || caster == null)
            return null;

        CharacterRaceAnimation raceId = ResolveRaceForVoice(caster, caster.RaceId);
        for (int i = 0; i < RaceSlots.Length && i < kinds.Length; i++)
        {
            if (RaceSlots[i] == raceId && !string.IsNullOrEmpty(kinds[i]))
                return kinds[i];
        }
        if (kinds.Length > 8 && !string.IsNullOrEmpty(kinds[8]))
            return kinds[8];
        for (int i = 0; i < kinds.Length; i++)
            if (!string.IsNullOrEmpty(kinds[i]))
                return kinds[i];
        return null;
    }

    static string VoicePrefixFor(Entity caster)
    {
        if (caster == null)
            return null;

        CharacterRaceAnimation raceId = ResolveRaceForVoice(caster, caster.RaceId);
        switch (raceId)
        {
            case CharacterRaceAnimation.MMagic: return "M_HMagician";
            case CharacterRaceAnimation.FMagic: return "F_HMagician";
            case CharacterRaceAnimation.MFighter: return "M_HFighter";
            case CharacterRaceAnimation.FFighter: return "F_HFighter";
            case CharacterRaceAnimation.MElf: return "M_Elf";
            case CharacterRaceAnimation.FElf: return "F_Elf";
            case CharacterRaceAnimation.MDarkElf: return "M_DarkElf";
            case CharacterRaceAnimation.FDarkElf: return "F_DarkElf";
            case CharacterRaceAnimation.MDwarf: return "M_Dwarf";
            case CharacterRaceAnimation.FDwarf: return "F_Dwarf";
            case CharacterRaceAnimation.MOrc: return "M_Orc_Fighter";
            case CharacterRaceAnimation.FOrc: return "F_Orc_Fighter";
            case CharacterRaceAnimation.MShaman: return "M_Orc_Magician";
            case CharacterRaceAnimation.FShaman: return "F_Orc_Magician";
            default: return null;
        }
    }
}
