using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static partial class EntitySpawnShared
{
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
            if (TryLoadNpcBody(models, package, candidate, out prefab, out usedMesh))
                return true;
        }

        // Dark elf village spells the race "Darkelf" and its traders have no A/B letter.
        // The imported bodies are trader B and the elf commoners.
        for (int i = 0; i < VillageBodyFallbacks.Length; i++)
        {
            if (!string.Equals(asset, VillageBodyFallbacks[i].From, System.StringComparison.Ordinal))
                continue;
            if (TryLoadNpcBody(models, package, VillageBodyFallbacks[i].To, out prefab, out usedMesh))
                return true;
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
            if (TryLoadNpcBody(models, package, candidate, out prefab, out usedMesh))
                return true;
            // Race swap on the substituted outfit as well (C + Elf → B + Human).
            for (int r = 0; r < FallbackRaceNames.Length; r++)
            {
                string raced = candidate;
                for (int m = 0; m < MissingRaceNames.Length; m++)
                    raced = raced.Replace(MissingRaceNames[m], FallbackRaceNames[r]);
                if (raced == candidate)
                    continue;
                if (TryLoadNpcBody(models, package, raced, out prefab, out usedMesh))
                    return true;
            }
        }

        // Do NOT substitute a random townsfolk / soldier mesh — that confuses Symbol Maker etc.
        return false;
    }

    static readonly (string From, string To)[] VillageBodyFallbacks =
    {
        ("a_trader_FDarkelf_m00", "a_traderB_FHuman_m00"),
        ("a_trader_FDarkElf_m00", "a_traderB_FHuman_m00"),
        ("a_trader_MDarkElf_m00", "a_traderB_MHuman_m00"),
        ("a_trader_MDarkelf_m00", "a_traderB_MHuman_m00"),
        ("a_common_people_FDarkElf_m00", "a_common_people_FElf_m00"),
        ("a_common_people_FDarkelf_m00", "a_common_people_FElf_m00"),
        ("a_common_people_MDarkelf_m00", "a_common_people_MElf_m00"),
        ("a_common_people_MDarkElf_m00", "a_common_people_MElf_m00"),
        ("a_guard_FDarkelf_m00", "a_guard_MHuman_m00"),
        ("a_guard_FDarkElf_m00", "a_guard_MHuman_m00"),
    };

    static readonly string[] NpcPackages = { "LineageNPCs", "LineageNPCs4", "LineageNpcs" };

    static bool TryLoadNpcBody(ModelTable models, string package, string asset, out GameObject prefab, out string usedMesh)
    {
        prefab = models.GetOrLoadNpc(package + "." + asset);
        if (prefab != null)
        {
            usedMesh = package + "." + asset;
            return true;
        }

        if (package.IndexOf("LineageNPC", System.StringComparison.OrdinalIgnoreCase) < 0)
        {
            usedMesh = null;
            return false;
        }

        for (int i = 0; i < NpcPackages.Length; i++)
        {
            if (string.Equals(NpcPackages[i], package, System.StringComparison.OrdinalIgnoreCase))
                continue;
            prefab = models.GetOrLoadNpc(NpcPackages[i] + "." + asset);
            if (prefab != null)
            {
                usedMesh = NpcPackages[i] + "." + asset;
                return true;
            }
        }

        usedMesh = null;
        return false;
    }

    static readonly (string From, string To)[] OutfitLetterFallbacks =
    {
        ("traderC_", "traderB_"),
        ("traderA_", "traderB_"),
        ("_traderC", "_traderB"),
    };
}
