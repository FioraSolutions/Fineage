using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EntitySpawnShared
{
    /// <summary>
    /// The client ships only part of the Interlude NPC art. Elven Village alone asks for
    /// <c>a_teleporter_FElf_m00</c> and <c>a_guard_MElf_m00</c>, which have no prefab, so the
    /// gatekeeper and the newbie guide were dropped on spawn while the server kept them alive.
    /// Rather than hide the NPC, substitute an existing rig in this order: same outfit on a
    /// shipped race, then the same race's generic townsfolk body.
    /// </summary>
    static bool TryResolveFallbackNpcPrefab(
        ModelTable models,
        string wantedMesh,
        out GameObject prefab,
        out string usedMesh)
    {
        prefab = null;
        usedMesh = null;
        if (models == null || string.IsNullOrEmpty(wantedMesh))
            return false;

        int dot = wantedMesh.IndexOf('.');
        if (dot <= 0 || dot == wantedMesh.Length - 1)
            return false;

        string package = wantedMesh.Substring(0, dot);
        string asset = wantedMesh.Substring(dot + 1);

        for (int i = 0; i < FallbackRaceNames.Length; i++)
        {
            string candidate = asset;
            for (int r = 0; r < MissingRaceNames.Length; r++)
                candidate = candidate.Replace(MissingRaceNames[r], FallbackRaceNames[i]);
            if (candidate == asset)
                continue;
            prefab = models.GetNpc(package + "." + candidate);
            if (prefab != null)
            {
                usedMesh = package + "." + candidate;
                return true;
            }
        }

        // Outfit-letter stand-in: Symbol Maker (31046) asks for a_traderC_* but the art pack
        // only ships a_traderB_*. Same trader silhouette, not a random townsfolk body.
        for (int i = 0; i < OutfitLetterFallbacks.Length; i++)
        {
            string from = OutfitLetterFallbacks[i].From;
            string to = OutfitLetterFallbacks[i].To;
            if (asset.IndexOf(from, System.StringComparison.OrdinalIgnoreCase) < 0)
                continue;
            string candidate = ReplaceIgnoreCase(asset, from, to);
            if (candidate == asset)
                continue;
            prefab = models.GetNpc(package + "." + candidate);
            if (prefab != null)
            {
                usedMesh = package + "." + candidate;
                return true;
            }
            // Race swap on the substituted outfit as well (C + Elf → B + Human).
            for (int r = 0; r < FallbackRaceNames.Length; r++)
            {
                string raced = candidate;
                for (int m = 0; m < MissingRaceNames.Length; m++)
                    raced = raced.Replace(MissingRaceNames[m], FallbackRaceNames[r]);
                if (raced == candidate)
                    continue;
                prefab = models.GetNpc(package + "." + raced);
                if (prefab != null)
                {
                    usedMesh = package + "." + raced;
                    return true;
                }
            }
        }

        // Do NOT substitute a random townsfolk / soldier mesh — that confuses Symbol Maker etc.
        return false;
    }

    static readonly (string From, string To)[] OutfitLetterFallbacks =
    {
        ("traderC_", "traderB_"),
        ("traderA_", "traderB_"),
        ("_traderC", "_traderB"),
    };

    static string ReplaceIgnoreCase(string text, string from, string to)
    {
        int idx = text.IndexOf(from, System.StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return text;
        return text.Substring(0, idx) + to + text.Substring(idx + from.Length);
    }

    /// <summary>
    /// Monster rigs live in <c>LineageMonsters*</c>; townsfolk rigs live in <c>LineageNPCs*</c>. The
    /// package prefix is what decides which stand-in table applies, so this stays a pure string test
    /// and works even when no prefab resolved at all.
    /// </summary>
    static bool IsMonsterMesh(string mesh) =>
        !string.IsNullOrEmpty(mesh) &&
        mesh.StartsWith("LineageMonster", System.StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Monster rigs the art pack does not ship. Elven Village is the loud case: Dryads, Lireins and
    /// Spore Fungus have no prefab, so the server kept them alive while the client dropped every row
    /// from the knownlist ("not all the mobs around Elven Village"). Substituting a shipped rig by
    /// silhouette keeps the mob visible, targetable and killable; the alternative — hiding it — is
    /// what made the area look empty.
    /// </summary>
    static bool TryResolveFallbackMonsterPrefab(
        ModelTable models,
        string wantedMesh,
        out GameObject prefab,
        out string usedMesh)
    {
        prefab = null;
        usedMesh = null;
        if (models == null || string.IsNullOrEmpty(wantedMesh))
            return false;

        int dot = wantedMesh.IndexOf('.');
        if (dot <= 0 || dot == wantedMesh.Length - 1)
            return false;

        string package = wantedMesh.Substring(0, dot);
        string asset = wantedMesh.Substring(dot + 1);

        // Exact rig first, so a mesh added to the art pack later wins over the stand-in.
        prefab = models.GetNpc(package + "." + asset);
        if (prefab != null)
        {
            usedMesh = package + "." + asset;
            return true;
        }

        string[] standIns = MonsterStandInMeshes(asset);
        for (int i = 0; i < standIns.Length; i++)
        {
            prefab = models.GetNpc("LineageMonsters." + standIns[i]);
            if (prefab != null)
            {
                usedMesh = "LineageMonsters." + standIns[i];
                return true;
            }
            prefab = models.GetNpc("LineageMonsters2." + standIns[i]);
            if (prefab != null)
            {
                usedMesh = "LineageMonsters2." + standIns[i];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Shipped rigs per silhouette family, most specific first. Both arrays are ordered, so the
    /// stand-in stays stable across runs and shows up once in the substitution log.
    /// </summary>
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

    /// <summary>Race tags the client art pack actually ships for NPC outfits, in preference order.</summary>
    static readonly string[] FallbackRaceNames =
    {
        "_FHuman_", "_MHuman_", "_FDwarf_", "_MDwarf_", "_FFighter_", "_MFighter_"
    };

    /// <summary>
    /// <c>e_clergyman_MOrc_m00</c> has no <c>_M</c> token; the old IndexOf('M')+_M check
    /// mis-classified Orcs as female and picked FElf townsfolk.
    /// </summary>
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
