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
/// Jobs: morc_anim | tile_scenes | bind_18_14 | orc_prefabs
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
}
#endif
