#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Rebuild only BSP brushes (Brushes.prefab + Brushes_Mesh.asset) for Interlude tiles from Meta/&lt;tile&gt;.t3d
/// (brush actors appended by prepare_interlude_tile.py / BrushDump.java), then rebuild the tile scene.
/// Unity -batchmode -nographics -quit -executeMethod L2InterludeBrushesCli.RunCli
/// Env: FINEAGE_INTERLUDE_TILES="20_19,20_20,21_19,21_20"
/// </summary>
public static class L2InterludeBrushesCli
{
    [MenuItem("Shnok/0.4. [Interlude] Rebuild BSP brushes (Elven quartet)")]
    public static void Menu() { Run("20_19,20_20,21_19,21_20", false); }

    public static void RunCli()
    {
        string tiles = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILES") ?? "20_19,20_20,21_19,21_20";
        bool ok = Run(tiles, true);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static string Rebuild(string tile)
    {
        bool ok = Run(tile, true);
        return ok ? "brushes=ok" : "brushes=fail";
    }

    static bool Run(string tiles, bool scenes)
    {
        bool ok = true;
        foreach (var raw in tiles.Split(','))
        {
            string tile = raw.Trim();
            if (tile.Length == 0) continue;
            string folder = "Assets/Resources/Data/Maps/" + tile;
            string t3d = Path.GetFullPath(folder + "/Meta/" + tile + ".t3d");
            if (!File.Exists(t3d)) { Debug.LogError("[BrushesCli] missing " + t3d); ok = false; continue; }
            GameObject root = L2BrushBuilder.BuildBrushesFromT3d(t3d);
            string prefab = folder + "/Brushes.prefab";
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefab) != null) AssetDatabase.DeleteAsset(prefab);
            PrefabUtility.SaveAsPrefabAsset(root, prefab);
            UnityEngine.Object.DestroyImmediate(root);
            Debug.Log("[BrushesCli] " + tile + " -> " + prefab);
            if (scenes && !L2InterludeTileSceneCli.BuildScene(tile)) { Debug.LogError("[BrushesCli] scene build failed " + tile); ok = false; }
        }
        AssetDatabase.SaveAssets();
        return ok;
    }
}
#endif
