#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Confirms a map tile's source of truth, Resources/Data/Maps/{XX_YY}/{XX_YY}.prefab.
/// Does not write Assets/Resources/Scenes/{XX_YY}.unity. A missing prefab throws
/// <see cref="MapTileMissingException"/>.
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

    [MenuItem("Shnok/0.2. [Interlude] Verify tile prefab")]
    static void MenuBuild()
    {
        string tile = EditorUtility.DisplayDialogComplex("Tile", "Use FINEAGE_INTERLUDE_TILES env or 21_19", "21_19", "Cancel", "") == 0 ? "21_19" : null;
        if (tile != null) BuildScene(tile);
    }

    public static bool BuildScene(string tile)
    {
        string prefabPath = MapsRoot + "/" + tile + "/" + tile + ".prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
            throw MapTileLoadPolicy.Missing(tile);

        string scenePath = ScenesRoot + "/" + tile + ".unity";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
            AssetDatabase.DeleteAsset(scenePath);

        var kept = EditorBuildSettings.scenes.Where(s => s.path != scenePath).ToArray();
        if (kept.Length != EditorBuildSettings.scenes.Length)
        {
            EditorBuildSettings.scenes = kept;
            AssetDatabase.SaveAssets();
        }

        Debug.Log("[Tile] " + tile + " source " + prefabPath);
        return true;
    }
}
#endif
