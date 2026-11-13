// fineage-touch 1794515200.orcscale
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// While the Editor is already open (batchmode cannot attach), drop a jobs file to run
/// one-shot import/bake steps after domain reload.
/// Flag: Assets/Editor/FineagePendingEditorJobs.txt — one job name per line.
/// Jobs: morc_anim | tile_scenes | bind_18_14 | orc_prefabs | orc_npc | shaman_faces | casino_npc | elf_teleporter_npc | darkelf_npc | npc_bounds | light_elf_vfx | bake_tile XX_YY | rebuild_static XX_YY | rebuild_brushes XX_YY
/// </summary>
[InitializeOnLoad]
static class FineagePendingEditorJobs
{
    const string FlagPath = "Assets/Editor/FineagePendingEditorJobs.txt";
    static bool _running;

    /// <summary>
    /// Batch: -executeMethod FineagePendingEditorJobs.RunRebuildStaticCli
    /// Env: FINEAGE_INTERLUDE_TILE=17_22 (static meshes only — safe in -nographics).
    /// </summary>
    public static void RunRebuildStaticCli()
    {
        string tile = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILE");
        if (string.IsNullOrWhiteSpace(tile))
            tile = "17_22";
        string result = RebuildStatic(tile.Trim());
        Debug.Log("[FineagePendingEditorJobs] rebuild_static " + tile + ": " + result);
        if (Application.isBatchMode)
            EditorApplication.Exit(result.Contains("static=ok") ? 0 : 1);
    }

    static FineagePendingEditorJobs()
    {
        EditorApplication.delayCall += TryConsume;
        EditorApplication.playModeStateChanged += _ => EditorApplication.delayCall += TryConsume;
    }

    static void TryConsume()
    {
        if (_running || EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        if (!File.Exists(FlagPath))
            return;

        string[] jobs = File.ReadAllLines(FlagPath);
        File.Delete(FlagPath);
        AssetDatabase.Refresh();

        _running = true;
        try
        {
            var log = new List<string>();
            foreach (string raw in jobs)
            {
                string job = (raw ?? "").Trim();
                if (job.Length == 0 || job.StartsWith("#"))
                    continue;
                try
                {
                    log.Add(job + ": " + RunJob(job));
                }
                catch (Exception ex)
                {
                    log.Add(job + " FAIL: " + ex.Message);
                    Debug.LogException(ex);
                }
            }
            Debug.Log("[FineagePendingEditorJobs] " + string.Join(" || ", log));
        }
        finally
        {
            _running = false;
        }
    }

    static string RunJob(string job)
    {
        if (job.StartsWith("bake_tile ", StringComparison.Ordinal))
            return BakeTile(job.Substring("bake_tile ".Length).Trim());
        if (job.StartsWith("reconvert_terrain ", StringComparison.Ordinal))
            return ReconvertTerrain(job.Substring("reconvert_terrain ".Length).Trim());
        if (job.StartsWith("rebuild_static ", StringComparison.Ordinal))
            return RebuildStatic(job.Substring("rebuild_static ".Length).Trim());
        if (job.StartsWith("rebuild_brushes ", StringComparison.Ordinal))
            return L2InterludeBrushesCli.Rebuild(job.Substring("rebuild_brushes ".Length).Trim());

        switch (job)
        {
            case "morc_anim":
                return L2MOrcAnimImporter.Run();
            case "orc_prefabs":
                L2OrcRacePrefabBuilder.BuildMOrcOnly();
                return "ok";
            case "orc_npc":
                return L2OrcNpcPrefabBuilder.Run();
            case "shaman_faces":
                return L2OrcRacePrefabBuilder.RebuildShamanFaces();
            case "casino_npc":
                return L2CasinoNpcPrefabBuilder.Run();
            case "elf_teleporter_npc":
                return L2ElfTeleporterNpcPrefabBuilder.Run();
            case "darkelf_npc":
                return L2DarkElfNpcPrefabBuilder.Run();
            case "npc_bounds":
                return L2NpcPrefabBoundsDiag.Run();
            case "light_elf_vfx":
            {
                bool ok = L2EffectGeneratorWindow.TryRunLightElfBatch(out string summary);
                Debug.Log("[FineagePendingEditorJobs] " + summary);
                return ok ? "ok" : summary;
            }
            case "tile_scenes":
            {
                string[] tiles = { "18_14", "19_14", "19_15", "20_17", "20_18", "23_11", "23_12", "24_11", "24_12" };
                int n = 0;
                foreach (string t in tiles)
                    if (L2InterludeTileSceneCli.BuildScene(t)) n++;
                return "scenes " + n + "/" + tiles.Length;
            }
            case "bind_18_14":
            {
                var log = new List<string>();
                string repo = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
                string export = Path.Combine(repo, ".work", "umodel-export-18_14");
                L2InterludeMaterialBinder.Bind(
                    Directory.Exists(export) ? new[] { export } : null,
                    log);
                foreach (var l in log) Debug.Log("[MatBinder] " + l);
                return string.Join("; ", log);
            }
            default:
                return "unknown job";
        }
    }

    /// <summary>
    /// Re-place StaticMeshes.prefab + streamable scene from already-imported FBX (no terrain re-bake).
    /// </summary>
    static string RebuildStatic(string tile)
    {
        if (string.IsNullOrWhiteSpace(tile) || tile.IndexOf('_') < 0)
            return "bad tile id";

        string repo = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
        string export = Path.Combine(repo, ".work", "umodel-export-" + tile);
        if (Directory.Exists(export))
        {
            Environment.SetEnvironmentVariable(L2InterludeClientSettings.EnvUModelExportRoot, export);
            L2InterludeClientSettings.UModelExportRoot = export;
        }

        AssetDatabase.Refresh();
        L2MapStaticMeshBuilder.MeshLocalYawFix = 90f;
        try
        {
            L2TerrainGeneratorTool.GenerateStaticMeshesForMap(tile);
        }
        finally
        {
            L2MapStaticMeshBuilder.MeshLocalYawFix = 0f;
        }
        L2InterludeTileBakePipeline.SaveNamedScenePrefab(tile, "StaticMeshes", "StaticMeshes.prefab");
        bool sceneOk = L2InterludeTileSceneCli.BuildScene(tile);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return "static=ok scene=" + (sceneOk ? "ok" : "fail");
    }

    /// <summary>
    /// Unity pipeline + tile scene for one square (Phase A / meshes must already be on disk).
    /// Must run in the open Editor — batchmode/-nographics garbles terrain Texture2DArrays.
    /// </summary>
    /// <summary>GPU rebake of one tile's heightmap mesh and splat arrays. Not safe in -nographics.</summary>
    static string ReconvertTerrain(string tile)
    {
        if (string.IsNullOrWhiteSpace(tile) || tile.IndexOf('_') < 0)
            return "bad tile id";
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            return "no GPU — would write 0xCD textures";

        bool ok = TerrainConverter.ConvertTerrainToMeshForMap(tile);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return ok ? "terrain=ok" : "terrain=fail";
    }

    static string BakeTile(string tile)
    {
        if (string.IsNullOrWhiteSpace(tile) || tile.IndexOf('_') < 0)
            return "bad tile id";

        string repo = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
        string export = Path.Combine(repo, ".work", "umodel-export-" + tile);
        string clientGuess = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "Downloads/L2_Interlude_Client/Lineage II");
        string client = Environment.GetEnvironmentVariable(L2InterludeClientSettings.EnvClientRoot);
        if (string.IsNullOrEmpty(client))
            client = L2InterludeClientSettings.ClientRoot;
        if (string.IsNullOrEmpty(client) || !Directory.Exists(Path.Combine(client, "maps")))
            client = clientGuess;

        // Getter prefers process env over EditorPrefs — force both so a stale
        // L2_UMODEL_EXPORT_DIR from another tile cannot steal the import root.
        if (Directory.Exists(export))
        {
            Environment.SetEnvironmentVariable(L2InterludeClientSettings.EnvUModelExportRoot, export);
            L2InterludeClientSettings.UModelExportRoot = export;
        }
        Environment.SetEnvironmentVariable(L2InterludeClientSettings.EnvClientRoot, client);
        L2InterludeClientSettings.ClientRoot = client;

        var steps = L2InterludeTileBakePipeline.DefaultSteps | L2InterludeBakeStep.GenerateMaterials;
        var result = L2InterludeTileBakePipeline.RunSingleTile(tile, steps);
        foreach (string line in result.log)
            Debug.Log("[bake_tile " + tile + "] " + line);

        var matLog = new List<string>();
        L2InterludeMaterialBinder.Bind(Directory.Exists(export) ? new[] { export } : null, matLog);
        foreach (string line in matLog)
            Debug.Log("[bake_tile " + tile + "] materials: " + line);

        bool sceneOk = false;
        if (result.success)
            sceneOk = L2InterludeTileSceneCli.BuildScene(tile);

        L2InterludeTileManifest.WriteToRepo(L2InterludeTileManifest.Build());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return "pipeline=" + (result.success ? "ok" : "fail")
            + " scene=" + (sceneOk ? "ok" : "fail")
            + " mats=" + matLog.Count;
    }
}
#endif

