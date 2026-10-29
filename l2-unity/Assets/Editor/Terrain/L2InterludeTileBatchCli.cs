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
}
#endif
