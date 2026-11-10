// fineage-touch 1794037730.455447
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
/// Jobs: morc_anim | tile_scenes | bind_18_14 | orc_prefabs | bake_tile XX_YY
/// </summary>
[InitializeOnLoad]
static class FineagePendingEditorJobs
{
    const string FlagPath = "Assets/Editor/FineagePendingEditorJobs.txt";
    static bool _running;

    static FineagePendingEditorJobs()
    {
        EditorApplication.delayCall += TryConsume;
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

        switch (job)
        {
            case "morc_anim":
                return L2MOrcAnimImporter.Run();
            case "orc_prefabs":
                L2OrcRacePrefabBuilder.BuildMOrcOnly();
                return "ok";
            case "orc_npc":
                return L2OrcNpcPrefabBuilder.Run();
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
    /// Unity pipeline + tile scene for one square (Phase A / meshes must already be on disk).
    /// Must run in the open Editor — batchmode/-nographics garbles terrain Texture2DArrays.
    /// </summary>
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

        if (Directory.Exists(export))
            L2InterludeClientSettings.UModelExportRoot = export;
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

