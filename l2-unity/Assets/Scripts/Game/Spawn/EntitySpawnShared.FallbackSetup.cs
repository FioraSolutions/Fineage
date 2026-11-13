using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static partial class EntitySpawnShared
{
    /// <summary>Race tags used inside LineageNPCs mesh names that the client art pack omits.</summary>
    static readonly string[] MissingRaceNames =
    {
        "_FElf_", "_MElf_", "_FDarkElf_", "_MDarkElf_", "_FDarkelf_", "_MDarkelf_",
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
