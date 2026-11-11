using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static partial class EntitySpawnShared
{
    static string[] MonsterStandInMeshes(string asset)
    {
        // Tall treant / ti_mi_tran (~90 collision height): stone golem is the only shipped bulky rig.
        if (Has(asset, "ti_mi_tran") || Has(asset, "ti_mi_riran") || Has(asset, "treant"))
            return new[] { "stone_golem_m00", "orc_fighter_m00", "goblin_m00" };

        // Dryad / lirein-sized tree spirits (~15 height): golem was absurd; use a small humanoid.
        if (Has(asset, "dryad") || Has(asset, "lirein"))
            return new[] { "orc_fighter_m00", "goblin_m00", "elpy_m00" };

        // Fungus / slime family: short, wide, no weapon.
        if (Has(asset, "fungus") || Has(asset, "slime") || Has(asset, "mushroom"))
            return new[] { "poison_toad_m00", "elpy_m00", "goblin_m00" };

        // Winged spirit family (Pixy / Sylph): prefer elpy over hatchling (less dragon-like).
        if (Has(asset, "sylph") || Has(asset, "pixy") || Has(asset, "fairy") || Has(asset, "spirit"))
            return new[] { "elpy_m00", "hatchling_m00", "wyvern_m00" };

        // Biped orc / goblinoid brute.
        if (Has(asset, "orc") || Has(asset, "goblin") || Has(asset, "bugbear") || Has(asset, "ogre"))
            return new[] { "orc_m00", "orc_fighter_m00", "goblin_m00" };

        // Arachnid — prefer real aracne_m00 (Nerupa) once imported; giant_spider is legacy stand-in.
        if (Has(asset, "aracne"))
            return new[] { "aracne_m00", "giant_spider_m00" };
        if (Has(asset, "spider"))
            return new[] { "giant_spider_m00", "aracne_m00" };

        // Canine / feline quadruped.
        if (Has(asset, "wolf") || Has(asset, "keltir") || Has(asset, "fox") || Has(asset, "cat"))
            return new[] { "wolf_m00", "fox_m00", "keltir_m00" };

        // Undead.
        if (Has(asset, "zombie") || Has(asset, "skeleton") || Has(asset, "ghoul") || Has(asset, "vampire"))
            return new[] { "werewolf_bi_m00", "werewolf_m00" };

        // Anything else still needs a body; a generic humanoid brute is the least surprising.
        return new[] { "goblin_m00", "orc_m00" };
    }

    static bool Has(string text, string needle) =>
        text.IndexOf(needle, System.StringComparison.OrdinalIgnoreCase) >= 0;

    /// <summary>Race tags used inside LineageNPCs mesh names that the client art pack omits.</summary>
    static readonly string[] MissingRaceNames =
    {
        "_FElf_", "_MElf_", "_FDarkElf_", "_MDarkElf_",
        "_FOrc_", "_MOrc_", "_FDwarf_", "_MDwarf_"
    };

    static readonly string[] FallbackRaceNames =
    {
        "_FHuman_", "_MHuman_", "_FDwarf_", "_MDwarf_", "_FFighter_", "_MFighter_"
    };

    static bool IsMaleNpcAsset(string asset)
    {
        if (string.IsNullOrEmpty(asset))
            return true;
        if (ContainsRaceTag(asset, 'F'))
            return false;
        if (ContainsRaceTag(asset, 'M'))
            return true;
        return asset.IndexOf('M') >= 0 && asset.Contains("_M");
    }

    static bool ContainsRaceTag(string asset, char sex)
    {
        // _MOrc_ / _FHuman_ / _MDarkElf_ and trailing _MOrc_m00 forms.
        string[] races = { "Orc", "Elf", "DarkElf", "Human", "Dwarf", "Fighter", "Magic" };
        for (int i = 0; i < races.Length; i++)
        {
            string tag = "_" + sex + races[i];
            if (asset.IndexOf(tag, System.StringComparison.Ordinal) >= 0)
                return true;
        }
        return false;
    }
}
