#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Builds a streamable world scene Assets/Resources/Scenes/{XX_YY}.unity from baked tile prefabs
/// and registers it in Build Settings so SceneLoader can stream it (#559 / Elven Village 21_19).
/// Unity -batchmode -nographics -quit -executeMethod L2InterludeTileSceneCli.RunCli
/// Env: FINEAGE_INTERLUDE_TILES=21_19[,XX_YY...]
/// </summary>
public static class L2InterludeTileSceneCli
{
    const string MapsRoot = "Assets/Resources/Data/Maps";
    const string ScenesRoot = "Assets/Resources/Scenes";

    public static void RunCli()
    {
        string raw = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILES") ?? "21_19";
        bool ok = true;
        foreach (string tile in raw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()))
        {
            ok &= BuildScene(tile);
        }
        EditorApplication.Exit(ok ? 0 : 1);
    }

    [MenuItem("Shnok/0.2. [Interlude] Build tile scene + add to Build Settings")]
    static void MenuBuild()
    {
        string tile = EditorUtility.DisplayDialogComplex("Tile", "Use FINEAGE_INTERLUDE_TILES env or 21_19", "21_19", "Cancel", "") == 0 ? "21_19" : null;
        if (tile != null) BuildScene(tile);
    }

    public static bool BuildScene(string tile)
    {
        string dir = MapsRoot + "/" + tile;
        string[] parts = { tile + ".prefab", tile + "_DecoLayer.prefab", "StaticMeshes.prefab", "Brushes.prefab", tile + "_AmbientSounds.prefab" };
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        int placed = 0;
        foreach (string p in parts)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(dir + "/" + p);
            if (prefab == null)
            {
                Debug.LogWarning("[Tile scene] missing " + dir + "/" + p);
                continue;
            }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            go.transform.SetPositionAndRotation(prefab.transform.position, prefab.transform.rotation);
            placed++;
        }
        if (AssetDatabase.LoadAssetAtPath<GameObject>(dir + "/" + tile + ".prefab") == null)
        {
            Debug.LogError("[Tile scene] terrain prefab missing for " + tile);
            return false;
        }
        Directory.CreateDirectory(ScenesRoot);
        string scenePath = ScenesRoot + "/" + tile + ".unity";
        EditorSceneManager.SaveScene(scene, scenePath);

        var list = EditorBuildSettings.scenes.ToList();
        if (!list.Any(s => s.path == scenePath))
        {
            list.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = list.ToArray();
        }
        AssetDatabase.SaveAssets();
        Debug.Log("[Tile scene] " + scenePath + " placed=" + placed);
        return true;
    }
}
#endif
