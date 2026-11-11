using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
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
