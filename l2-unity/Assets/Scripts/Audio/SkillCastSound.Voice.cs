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

    static CharacterRaceAnimation ResolveRaceForVoice(Entity caster, CharacterRaceAnimation raceId)
    {
        PlayerAppearance appearance = caster.Appearance as PlayerAppearance;
        if (appearance == null)
            return raceId;

        int playerClass = caster.Identity != null ? caster.Identity.PlayerClass : 0;
        CharacterRace race = (CharacterRace)appearance.Race;
        CharacterRaceAnimation resolved = CharacterRaceAnimationResolve.ResolveRaceAnimation(
            race, appearance, playerClass);
        bool resolvedMage =
            resolved == CharacterRaceAnimation.MMagic || resolved == CharacterRaceAnimation.FMagic;
        bool looksUnset = raceId == CharacterRaceAnimation.FElf && race != CharacterRace.Elf;
        if (resolvedMage || looksUnset)
        {
            if (raceId != resolved)
                caster.RaceId = resolved;
            return resolved;
        }
        return raceId;
    }

    static string VoiceKindFor(string skillKey, Entity caster)
    {
        bool orcOrDwarf =
            caster != null && (
                caster.RaceId == CharacterRaceAnimation.MDwarf ||
                caster.RaceId == CharacterRaceAnimation.FDwarf ||
                caster.RaceId == CharacterRaceAnimation.MOrc ||
                caster.RaceId == CharacterRaceAnimation.FOrc ||
                caster.RaceId == CharacterRaceAnimation.MShaman ||
                caster.RaceId == CharacterRaceAnimation.FShaman);

        string k = skillKey ?? "";
        string kind;
        if (ContainsAny(k, "heal", "cure", "bless", "holy", "recharge", "vitalize", "restore",
                "purify", "peace", "greater_heal", "group_heal", "battle_heal"))
            kind = "White";
        else if (ContainsAny(k, "poison", "curse", "death", "dark", "sleep", "fear", "silence",
                "bleed", "anchor", "hex", "drain", "vampiric", "corpse", "soul"))
            kind = "Black";
        else if (ContainsAny(k, "summon", "servitor", "cubic"))
            kind = "Call";
        else if (ContainsAny(k, "fire", "flame", "ice", "frost", "wind", "storm", "water", "aqua",
                "bolt", "blaze", "flare", "burn", "hydro", "tempest", "twister", "prominence",
                "element", "aura_flare", "aura_burn", "hurricane"))
            kind = "Element";
        else if (ContainsAny(k, "might", "shield", "acumen", "focus", "haste", "guidance",
                "empower", "wild_magic", "concentration", "decrease", "slow", "buff", "song",
                "dance", "chant", "blessed"))
            kind = "Sub";
        else if (ContainsAny(k, "throw", "spear", "bow"))
            kind = "Throw";
        else
            kind = "Shot";

        if (!orcOrDwarf)
            return kind;

        switch (kind)
        {
            case "White": return "Type_A";
            case "Black":
            case "Element": return "Type_B";
            case "Call":
            case "Sub": return "Type_C";
            default: return kind;
        }
    }

    static bool ContainsAny(string hay, params string[] needles)
    {
        for (int i = 0; i < needles.Length; i++)
            if (hay.IndexOf(needles[i]) >= 0)
                return true;
        return false;
    }

    static bool PlaySfx(string clipName, Vector3 pos, float volume)
    {
        AudioClip clip = Load(SfxRoot, clipName);
        if (clip == null && !string.IsNullOrEmpty(clipName))
            clip = Load(SfxRoot, clipName.ToLowerInvariant());
        if (clip == null)
            return false;
        InterludeSfx.Play(clip, volume);
        return true;
    }

}
