#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Rebuild only StaticMeshes.prefab (+ colliders + tile scene) for Interlude tiles from Meta/&lt;tile&gt;.t3d.
/// Unity -batchmode -nographics -quit -executeMethod L2InterludeStaticMeshesCli.RunCli
/// Env: FINEAGE_INTERLUDE_TILES="20_19,20_20,21_19,21_20"
/// </summary>
public static class L2InterludeStaticMeshesCli
{
    public static void RunCli()
    {
        string tiles = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILES") ?? "20_19,20_20,21_19,21_20";
        var list = tiles.Split(',').Select(t => t.Trim()).Where(t => t.Length > 0).ToList();
        bool ok = true;
        foreach (var r in L2InterludeTileBakePipeline.RunTiles(list, L2InterludeBakeStep.GenerateStaticMeshScene, false))
            Debug.Log("[StaticMeshesCli] " + string.Join(" | ", r.log));
        Debug.Log("[StaticMeshesCli] colliders: " + L2InterludeStaticMeshColliders.Run(list));
        // Water quads live as child "L2WaterVolumes" of StaticMeshes.prefab; a fresh prefab drops them.
        Debug.Log("[StaticMeshesCli] " + L2InterludeWaterVolumes.Run(list));
        foreach (var t in list)
            if (!L2InterludeTileSceneCli.BuildScene(t)) { Debug.LogError("[StaticMeshesCli] scene failed " + t); ok = false; }
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(ok ? 0 : 1);
    }
}
#endif
