#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Batchmode entry for Interlude tile bake (#476).
/// Unity -batchmode -nographics -quit
///   -executeMethod L2InterludeTileBatchCli.RunTileBakeCli
/// Env: L2_INTERLUDE_CLIENT_ROOT, L2_UMODEL_EXPORT_DIR, FINEAGE_INTERLUDE_TILE (default 22_22)
/// </summary>
public static class L2InterludeTileBatchCli
{
    public static void RunTileBakeCli()
    {
        string tileId = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILE") ?? "22_22";
        string clientRoot = Environment.GetEnvironmentVariable(L2InterludeClientSettings.EnvClientRoot);
        string exportRoot = Environment.GetEnvironmentVariable(L2InterludeClientSettings.EnvUModelExportRoot);

        if (!string.IsNullOrEmpty(clientRoot))
        {
            L2InterludeClientSettings.ClientRoot = clientRoot;
        }

        if (!string.IsNullOrEmpty(exportRoot))
        {
            L2InterludeClientSettings.UModelExportRoot = exportRoot;
        }

        if (!L2InterludeClientSettings.TryValidateUModelExportRoot(out string exportMsg))
        {
            Debug.LogError("[Interlude bake] " + exportMsg);
            EditorApplication.Exit(1);
            return;
        }

        L2InterludeBakeStep steps =
            L2InterludeTileBakePipeline.DefaultSteps | L2InterludeBakeStep.GenerateMaterials;

        List<L2InterludeTileBakeResult> results =
            L2InterludeTileBakePipeline.RunTiles(new[] { tileId }, steps, true);

        // Same post-step as Auto-Bake window: bind StaticMesh textures + water/translucent fix (Shnok 0.1/0.2).
        var matLog = new List<string>();
        string er = L2InterludeClientSettings.UModelExportRoot;
        L2InterludeMaterialBinder.Bind(!string.IsNullOrEmpty(er) && System.IO.Directory.Exists(er) ? new[] { er } : null, matLog);
        foreach (string l in matLog) Debug.Log("[Interlude bake] materials: " + l);

        L2InterludeTileManifest.WriteToRepo(L2InterludeTileManifest.Build());

        foreach (L2InterludeTileBakeResult result in results)
        {
            Debug.Log("[Interlude bake] tile=" + result.tileId + " success=" + result.success);
            foreach (string line in result.log)
            {
                Debug.Log("  " + line);
            }
        }

        bool ok = results.All(r => r.success);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    /// <summary>
    /// Terrain, brushes, and a streamable scene only. Skips the global material
    /// rebuild and static-mesh import so a neighbor tile can be walked onto
    /// before its props are exported.
    /// Env: FINEAGE_INTERLUDE_TILES=18_13,18_15
    /// </summary>
    public static void RunTerrainExitCli()
    {
        string raw = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILES") ?? "";
        string[] tiles = raw.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var steps =
            L2InterludeBakeStep.GenerateTerrain |
            L2InterludeBakeStep.BuildBrushes |
            L2InterludeBakeStep.ConvertTerrainToMesh |
            L2InterludeBakeStep.GenerateDecoLayerMesh;

        bool ok = tiles.Length > 0;
        foreach (string tile in tiles)
        {
            string id = tile.Trim();
            L2InterludeTileBakeResult result = L2InterludeTileBakePipeline.RunSingleTile(id, steps);
            Debug.Log("[ExitTile] " + id + " success=" + result.success);
            foreach (string line in result.log)
                Debug.Log("  " + line);
            bool scene = result.success && L2InterludeTileSceneCli.BuildScene(id);
            Debug.Log("[ExitTile] " + id + " scene=" + scene);
            ok &= result.success && scene;
        }

        L2InterludeTileManifest.WriteToRepo(L2InterludeTileManifest.Build());
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(ok ? 0 : 1);
    }
}
#endif
