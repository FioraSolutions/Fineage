#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InterludeTileBakePipeline
{
    static void SaveTerrainPrefab(string tileId, GameObject terrainGo)
    {
        string folder = Path.Combine(L2InterludeMapPaths.TileAssetFolder(tileId), "UnityTerrain").Replace("\\", "/");
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string absFolder = Path.Combine(projectRoot, folder);
        Directory.CreateDirectory(absFolder);
        string prefabPath = Path.Combine(folder, tileId + ".prefab").Replace("\\", "/");
        PrefabUtility.SaveAsPrefabAsset(terrainGo, prefabPath);
    }

    static void SaveScenePrefab(string tileId, GameObject root, string prefabFileName)
    {
        string folder = L2InterludeMapPaths.TileAssetFolder(tileId);
        string prefabPath = Path.Combine(folder, prefabFileName).Replace("\\", "/");
        // Break Prefab Variant chains between tiles (same hierarchy name "StaticMeshes").
        if (PrefabUtility.IsPartOfPrefabInstance(root))
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath) != null)
            AssetDatabase.DeleteAsset(prefabPath);
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        UnityEngine.Object.DestroyImmediate(root);
    }

    static void SaveScenePrefab(string tileId, string childName, string prefabFileName)
    {
        // Prefer densest root named childName — not an empty leftover Variant.
        GameObject best = null;
        int bestCount = -1;
        foreach (var t in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (t == null || t.parent != null || t.name != childName) continue;
            int n = t.GetComponentsInChildren<MeshFilter>(true).Length;
            if (n > bestCount) { bestCount = n; best = t.gameObject; }
        }
        if (best == null) return;
        SaveScenePrefab(tileId, best, prefabFileName);
    }
}
#endif
