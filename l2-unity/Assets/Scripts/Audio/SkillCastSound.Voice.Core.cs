using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    static readonly HashSet<string> VoiceMissLogged = new HashSet<string>(StringComparer.Ordinal);

    static void PlayVoiceKind(Entity caster, string kind, int skillId)
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
        // Same phase family only. The substitute is logged; it is not the skillsoundgrp cue.
        if (kind == "Notarget" && PlayVoiceClip(prefix + "_Shot"))
        {
            LogVoiceFallback(skillId, prefix, primary, prefix + "_Shot");
            return;
        }
        if (kind == "Shot" && PlayVoiceClip(prefix + "_Throw"))
        {
            LogVoiceFallback(skillId, prefix, primary, prefix + "_Throw");
            return;
        }
        LogVoiceFallback(skillId, prefix, primary, "");
    }

    static void LogVoiceFallback(int skillId, string raceSexPrefix, string expected, string played)
    {
        string key = skillId + "|" + expected + ">" + played;
        if (!VoiceMissLogged.Add(key))
            return;

        Debug.LogError(
            "[SkillCastSound] voice " + (string.IsNullOrEmpty(played) ? "missing" : "fallback") +
            " skillId=" + skillId +
            " raceSex=" + raceSexPrefix +
            " expected=" + expected +
            (string.IsNullOrEmpty(played) ? "" : " played=" + played));
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
