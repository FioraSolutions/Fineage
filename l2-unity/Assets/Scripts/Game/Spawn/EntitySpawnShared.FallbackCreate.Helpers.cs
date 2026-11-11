using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static partial class EntitySpawnShared
{
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
