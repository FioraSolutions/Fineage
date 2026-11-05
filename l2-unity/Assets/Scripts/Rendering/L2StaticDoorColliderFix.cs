using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Baked Interlude tiles ship door static meshes in their <b>closed</b> pose with a live
/// MeshCollider on layer StaticMesh. The client has no door open/close packets, so those
/// colliders wall off shop entrances (Elven Village grocery/weapon shops in 21_19 and the
/// same door sets on every other baked town).
///
/// This runtime pass disables colliders on door meshes only; visuals stay. Everything else in
/// the static mesh layer keeps its collider, so terrain and buildings still block the player.
/// </summary>
public static class L2StaticDoorColliderFix
{
    // Every door static mesh name found in Data/Maps/*/StaticMeshes.prefab, plus the families the
    // Interlude art uses. Matched as substrings against the instance GameObject name, which is
    // "Package.MeshName" (e.g. "Door_Set_S.ELF_Door_Set.Elf_Door_01").
    static readonly string[] DoorMeshMarkers =
    {
        "Elf_Door_",
        "H_Door_MV_",
        "H_Door_OP_",
        "_Door_01",
        "_Door_02",
    };

    static bool _installed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (_installed)
        {
            return;
        }

        _installed = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            ApplyScene(SceneManager.GetSceneAt(i));
        }
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyScene(scene);
    }

    static void ApplyScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        int disabled = 0;

        for (int r = 0; r < roots.Length; r++)
        {
            Collider[] colliders = roots[r].GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                if (collider == null || !collider.enabled)
                {
                    continue;
                }

                if (!IsDoorMesh(collider.gameObject.name))
                {
                    continue;
                }

                collider.enabled = false;
                disabled++;
            }
        }

        if (disabled > 0)
        {
            FineageLog.Info(
                FineageLog.Category.World,
                "L2StaticDoorColliderFix: disabled " + disabled + " door colliders in '" + scene.name + "'");
        }
    }

    static bool IsDoorMesh(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        for (int i = 0; i < DoorMeshMarkers.Length; i++)
        {
            if (name.IndexOf(DoorMeshMarkers[i], StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }
}
