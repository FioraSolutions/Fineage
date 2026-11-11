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

    static string ReplaceIgnoreCase(string text, string from, string to)
    {
        int idx = text.IndexOf(from, System.StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return text;
        return text.Substring(0, idx) + to + text.Substring(idx + from.Length);
    }

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
}
