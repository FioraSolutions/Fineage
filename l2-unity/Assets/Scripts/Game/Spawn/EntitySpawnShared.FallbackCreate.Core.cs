using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
