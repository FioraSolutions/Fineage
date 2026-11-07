#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// One-click Interlude tile auto-bake (Elven Village quartet by default).
/// MUST run inside an open Unity Editor (GPU): Texture2DArray / Blit / ReadPixels need a graphics device.
/// Per tile:
///   1. Phase A (shell)  : tools/interlude/run_mac_tile_bake.sh phase-a  -> Maps/XX_YY/Meta/*.t3d + .work/interlude-extract
///   2. Meshes (shell)   : run_mac_tile_bake.sh meshes  (umodel via Docker + Blender psk->fbx) -> .work/umodel-export-XX_YY
///   3. Unity pipeline   : L2InterludeTileBakePipeline.DefaultSteps (+materials once) with that export root
///   4. Scene            : L2InterludeTileSceneCli.BuildScene -> Assets/Resources/Scenes/XX_YY.unity + Build Settings
///   5. Diagnostics      : t3d StaticMeshActor count vs StaticMeshes.prefab, per-package FBX coverage, alphamap sanity.
/// Big generated assets stay local (no LFS); nothing is committed by this tool.
/// </summary>
public class L2InterludeAutoBakeWindow : EditorWindow
{
    const string DefaultTiles = "20_19,20_20,21_19,21_20";
    const string PrefTiles = "Fineage.AutoBake.Tiles";
    const string PrefClient = "Fineage.AutoBake.Client";

    string tiles;
    string clientRoot;
    bool runPhaseA = true;          // only when Meta/t3d or staticmeshes.json missing (unless force)
    bool runMeshes = true;          // only when FBX coverage < 100% (unless force)
    bool forceShell = false;
    bool runUnity = true;
    bool runScene = true;
    Vector2 scroll;
    readonly StringBuilder report = new StringBuilder();

    static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
    static string UnityRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
    static string MapsAbs => Path.Combine(Application.dataPath, "Resources/Data/Maps");
    static string StaticMeshesAbs => Path.Combine(Application.dataPath, "Resources/Data/StaticMeshes");

    [MenuItem("Shnok/0.0. [Interlude] Auto-bake tiles (Elven Village)…")]
    static void Open() => GetWindow<L2InterludeAutoBakeWindow>("Interlude Auto-Bake");

    void OnEnable()
    {
        tiles = EditorPrefs.GetString(PrefTiles, DefaultTiles);
        string guess = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Downloads/L2_Interlude_Client/Lineage II");
        clientRoot = EditorPrefs.GetString(PrefClient, FirstNonEmpty(L2InterludeClientSettings.ClientRoot, guess));
    }

    void OnGUI()
    {
        if (Application.isBatchMode || SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            EditorGUILayout.HelpBox("No GPU (batchmode/-nographics). Terrain textures would be garbage. Open the project in the Unity Editor.", MessageType.Error);
        }
        EditorGUILayout.HelpBox("Runs inside the open Editor (GPU). Shell steps need Docker running + Blender + Java (L2TerrainExtractor). Long: minutes per tile.", MessageType.Info);

        tiles = EditorGUILayout.TextField("Tiles (comma)", tiles);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Elven Village quartet")) tiles = DefaultTiles;
            if (GUILayout.Button("All with t3d")) tiles = string.Join(",", TilesWithT3d());
            if (GUILayout.Button("All .unr in client")) tiles = string.Join(",", TilesInClient(clientRoot));
        }
        clientRoot = EditorGUILayout.TextField("Interlude client root", clientRoot);
        runPhaseA = EditorGUILayout.Toggle("1. Phase A (t3d extract)", runPhaseA);
        runMeshes = EditorGUILayout.Toggle("2. Meshes (umodel+Blender)", runMeshes);
        forceShell = EditorGUILayout.Toggle("   force shell steps", forceShell);
        runUnity = EditorGUILayout.Toggle("3. Unity bake steps", runUnity);
        runScene = EditorGUILayout.Toggle("4. Build tile scene", runScene);

        using (new EditorGUI.DisabledScope(Application.isBatchMode))
        {
            if (GUILayout.Button("Diagnose only")) Run(diagnoseOnly: true);
            if (GUILayout.Button("BAKE", GUILayout.Height(32))) Run(diagnoseOnly: false);
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.TextArea(report.ToString(), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void Log(string s)
    {
        report.AppendLine(s);
        Debug.Log("[AutoBake] " + s);
        Repaint();
    }

    void Run(bool diagnoseOnly)
    {
        EditorPrefs.SetString(PrefTiles, tiles);
        EditorPrefs.SetString(PrefClient, clientRoot);
        report.Clear();
        if (!diagnoseOnly && (Application.isBatchMode || SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null))
        {
            Log("REFUSED: no GPU device. Run from the open Editor.");
            return;
        }

        string[] list = tiles.Split(new[] { ',', ' ', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).Distinct().ToArray();
        Log("Prereqs: " + Prereqs(clientRoot));
        bool materialsDone = false;
        try
        {
            for (int i = 0; i < list.Length; i++)
            {
                string tile = list[i];
                if (EditorUtility.DisplayCancelableProgressBar("Interlude Auto-Bake", tile, (float)i / list.Length)) { Log("Cancelled."); break; }
                if (diagnoseOnly) { Log(Diagnose(tile)); continue; }
                materialsDone |= BakeTile(tile, !materialsDone);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (!diagnoseOnly)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        Log("Done. Log also in Console.");
    }

    bool BakeTile(string tile, bool withMaterials)
    {
        Log("=== " + tile);
        string exportDir = Path.Combine(RepoRoot, ".work", "umodel-export-" + tile);
        string staticJson = Path.Combine(RepoRoot, ".work", "interlude-extract", tile, "staticmeshes.json");
        bool hasUnr = File.Exists(Path.Combine(clientRoot, "maps", tile + ".unr"));
        if (!hasUnr && !File.Exists(L2InterludeMapPaths.T3dAbsolutePath(tile)))
        {
            Log(tile + ": SKIP — no maps/" + tile + ".unr in client and no t3d");
            return false;
        }

        if (runPhaseA && (forceShell || !File.Exists(L2InterludeMapPaths.T3dAbsolutePath(tile)) || !File.Exists(staticJson)))
        {
            if (!Shell(tile, "phase-a", exportDir)) { Log(tile + ": FAIL phase-a (see .work/autobake-" + tile + "-phase-a.log)"); return false; }
        }
        if (runMeshes)
        {
            Coverage cov = MeshCoverage(tile, exportDir);
            if (forceShell || cov.missing > 0)
            {
                if (!File.Exists(staticJson)) Log(tile + ": WARN no staticmeshes.json — meshes step needs Phase A first");
                else if (!Shell(tile, "meshes", exportDir)) Log(tile + ": WARN meshes step failed (continuing with existing FBX)");
            }
        }

        bool ok = true;
        if (runUnity)
        {
            if (Directory.Exists(exportDir)) L2InterludeClientSettings.UModelExportRoot = exportDir;
            L2InterludeClientSettings.ClientRoot = clientRoot;
            var steps = L2InterludeTileBakePipeline.DefaultSteps;
            if (withMaterials)
            {
                steps |= L2InterludeBakeStep.GenerateMaterials;
                Log(tile + ": GenerateMaterials (global) — Editor may Import many .mat; watch Console");
            }
            var r = L2InterludeTileBakePipeline.RunSingleTile(tile, steps);
            foreach (string l in r.log) Log("  " + l);
            ok = r.success;
            Log(tile + ": pipeline " + (ok ? "OK" : "FAIL"));
            var mlog = new List<string>();
            L2InterludeMaterialBinder.Bind(Directory.Exists(exportDir) ? new[] { exportDir } : null, mlog);
            foreach (string l in mlog) Log("  materials: " + l);
        }
        if (runScene && ok)
        {
            bool s = L2InterludeTileSceneCli.BuildScene(tile);
            Log(tile + ": scene " + (s ? "OK" : "FAIL"));
        }
        Log(Diagnose(tile));
        return withMaterials && runUnity;
    }

    bool Shell(string tile, string phase, string exportDir)
    {
        string script = Path.Combine(RepoRoot, "tools/interlude/run_mac_tile_bake.sh");
        string logPath = Path.Combine(RepoRoot, ".work", "autobake-" + tile + "-" + phase + ".log");
        Directory.CreateDirectory(Path.GetDirectoryName(logPath));
        Log(tile + ": shell " + phase + " … (log " + logPath + ")");
        var psi = new ProcessStartInfo("/bin/bash", "-lc \"'" + script + "' " + phase + " > '" + logPath + "' 2>&1\"")
        {
            UseShellExecute = false,
            WorkingDirectory = RepoRoot,
        };
        psi.EnvironmentVariables["FINEAGE_INTERLUDE_TILE"] = tile;
        psi.EnvironmentVariables["L2_INTERLUDE_CLIENT_ROOT"] = clientRoot;
        psi.EnvironmentVariables["L2_UMODEL_EXPORT_DIR"] = exportDir;
        psi.EnvironmentVariables["PATH"] = "/opt/homebrew/bin:/usr/local/bin:/usr/bin:/bin:" + Environment.GetEnvironmentVariable("PATH");
        using (var p = Process.Start(psi))
        {
            while (!p.WaitForExit(500))
            {
                if (EditorUtility.DisplayCancelableProgressBar("Interlude Auto-Bake", tile + ": " + phase + " (shell)", 0.5f)) { try { p.Kill(); } catch { } return false; }
            }
            return p.ExitCode == 0;
        }
    }

    struct Coverage { public int needed, present, missing; public Dictionary<string, (int need, int have)> perPkg; }

    static Coverage MeshCoverage(string tile, string exportDir)
    {
        var cov = new Coverage { perPkg = new Dictionary<string, (int, int)>() };
        string t3d = L2InterludeMapPaths.T3dAbsolutePath(tile);
        if (!File.Exists(t3d)) return cov;
        var refs = Regex.Matches(File.ReadAllText(t3d), @"StaticMesh'([A-Za-z0-9_]+)\.(?:[A-Za-z0-9_]+\.)?([A-Za-z0-9_]+)'")
            .Cast<Match>().Select(m => (pkg: m.Groups[1].Value, mesh: m.Groups[2].Value)).Distinct();
        foreach (var (pkg, mesh) in refs)
        {
            bool have = File.Exists(Path.Combine(StaticMeshesAbs, pkg, mesh + ".fbx")) || File.Exists(Path.Combine(exportDir, pkg, mesh + ".fbx"));
            cov.needed++; if (have) cov.present++; else cov.missing++;
            cov.perPkg.TryGetValue(pkg, out var v);
            cov.perPkg[pkg] = (v.need + 1, v.have + (have ? 1 : 0));
        }
        return cov;
    }

    static string Diagnose(string tile)
    {
        var sb = new StringBuilder();
        string t3d = L2InterludeMapPaths.T3dAbsolutePath(tile);
        if (!File.Exists(t3d)) return tile + ": no t3d (needs Phase A)";
        string text = File.ReadAllText(t3d);
        int actors = Regex.Matches(text, "Begin Actor Class=StaticMeshActor").Count;
        string smPrefab = Path.Combine(MapsAbs, tile, "StaticMeshes.prefab");
        int placed = File.Exists(smPrefab) ? Regex.Matches(File.ReadAllText(smPrefab), "^MeshFilter:", RegexOptions.Multiline).Count : 0;
        var cov = MeshCoverage(tile, Path.Combine(RepoRoot, ".work", "umodel-export-" + tile));
        sb.AppendLine($"{tile}: t3d StaticMeshActors={actors}, StaticMeshes.prefab MeshFilters={placed}, unique meshes FBX {cov.present}/{cov.needed}");
        foreach (var kv in cov.perPkg.Where(k => k.Value.have < k.Value.need).OrderByDescending(k => k.Value.need - k.Value.have).Take(12))
            sb.AppendLine($"   missing FBX {kv.Key}: {kv.Value.need - kv.Value.have}/{kv.Value.need}");
        string mesh = Path.Combine(MapsAbs, tile, tile + "_Mesh.asset");
        sb.AppendLine("   terrain mesh: " + (File.Exists(mesh) ? "ok" : "MISSING"));
        foreach (string arr in new[] { "_Alphamap.asset", "_Layer.asset" })
        {
            string p = Path.Combine(MapsAbs, tile, tile + arr);
            sb.AppendLine("   " + tile + arr + ": " + (File.Exists(p) ? (ArrayLooksGarbage(p) ? "GARBAGE (0xCD fill) — re-run in Editor" : "ok") : "MISSING"));
        }
        string mat = Path.Combine(MapsAbs, tile, tile + "_Mat.mat");
        if (File.Exists(mat) && Regex.IsMatch(File.ReadAllText(mat), @"_Splatmaps:\s*\n\s*m_Texture: \{fileID: 0\}"))
            sb.AppendLine("   material: _Splatmaps EMPTY");
        bool collider = File.Exists(Path.Combine(MapsAbs, tile, tile + ".prefab")) && File.ReadAllText(Path.Combine(MapsAbs, tile, tile + ".prefab")).Contains("MeshCollider:");
        sb.Append("   ground collider: " + (collider ? "ok" : "MISSING"));
        return sb.ToString();
    }

    static bool ArrayLooksGarbage(string path)
    {
        using (var fs = File.OpenRead(path))
        {
            if (fs.Length < 4_000_000) return false;
            var buf = new byte[4096];
            var seen = new HashSet<byte>();
            for (long off = fs.Length / 4; off < fs.Length; off += fs.Length / 8)
            {
                fs.Position = off; int n = fs.Read(buf, 0, buf.Length);
                for (int i = 0; i < n; i++) seen.Add(buf[i]);
            }
            return seen.Count <= 3; // text-serialized "cdcd..." => only 'c','d'
        }
    }

    static string Prereqs(string client)
    {
        var parts = new List<string>();
        parts.Add(Directory.Exists(Path.Combine(client ?? "", "maps")) ? "client ok" : "client MISSING maps/");
        parts.Add(File.Exists(Path.Combine(UnityRoot, "External/UEViewer/umodel")) ? "umodel ok" : "umodel MISSING (l2-unity/External/UEViewer/umodel)");
        parts.Add(Directory.Exists(Path.Combine(UnityRoot, "External/L2TerrainExtractor")) ? "extractor ok" : "L2TerrainExtractor MISSING");
        parts.Add(File.Exists("/opt/homebrew/bin/blender") || File.Exists("/usr/local/bin/blender") ? "blender ok" : "blender MISSING (brew install --cask blender)");
        parts.Add(File.Exists("/usr/local/bin/docker") || File.Exists("/opt/homebrew/bin/docker") ? "docker cli ok (Docker Desktop must be running)" : "docker MISSING");
        return string.Join(", ", parts);
    }

    static IEnumerable<string> TilesWithT3d() =>
        Directory.GetDirectories(MapsAbs).Select(Path.GetFileName)
            .Where(t => Regex.IsMatch(t, @"^\d+_\d+$") && File.Exists(L2InterludeMapPaths.T3dAbsolutePath(t))).OrderBy(t => t);

    static IEnumerable<string> TilesInClient(string client)
    {
        string d = Path.Combine(client ?? "", "maps");
        if (!Directory.Exists(d)) return Enumerable.Empty<string>();
        return Directory.GetFiles(d, "*.unr").Select(Path.GetFileNameWithoutExtension).Where(t => Regex.IsMatch(t, @"^\d+_\d+$")).OrderBy(t => t);
    }

    static string FirstNonEmpty(params string[] v) => v.FirstOrDefault(s => !string.IsNullOrEmpty(s)) ?? "";
}
#endif
