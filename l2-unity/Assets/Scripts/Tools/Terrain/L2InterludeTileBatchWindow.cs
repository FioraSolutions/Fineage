#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class L2InterludeTileBatchWindow : EditorWindow
{
    string _allowlist = "16_24,16_25,17_24,17_25";
    string _denylist = string.Empty;
    string _clientRoot = string.Empty;
    string _umodelExport = string.Empty;
    string _umodelSoundExport = string.Empty;
    string _umodelSoundUnityExport = string.Empty;
    bool _includeGlobalMaterials = true;
    bool _refreshManifestAfterRun = true;
    L2InterludeBakeStep _steps = L2InterludeTileBakePipeline.DefaultSteps | L2InterludeBakeStep.GenerateMaterials;
    Vector2 _scroll;
    List<L2InterludeTileBakeResult> _lastResults = new List<L2InterludeTileBakeResult>();

    [MenuItem("Shnok/0. [Interlude] Batch tile bake (#476)...")]
    public static void Open()
    {
        L2InterludeTileBatchWindow window = GetWindow<L2InterludeTileBatchWindow>(true, "Interlude tile bake", true);
        window.minSize = new Vector2(520, 420);
        window.LoadPrefs();
    }

    [MenuItem("Shnok/0.1. [Interlude] Refresh tile manifest")]
    public static void RefreshManifestMenu()
    {
        L2InterludeTileManifestFile manifest = L2InterludeTileManifest.Build();
        L2InterludeTileManifest.WriteToRepo(manifest);
        EditorUtility.DisplayDialog(
            "Interlude tile manifest",
            $"Wrote {L2InterludeTileManifest.ManifestRepoPath}\n" +
            $"Client maps: {manifest.clientMapCount}\n" +
            $"Repo full: {manifest.importedFullCount}, partial: {manifest.importedPartialCount}",
            "OK");
    }

    void LoadPrefs()
    {
        _clientRoot = L2InterludeClientSettings.ClientRoot;
        _umodelExport = L2InterludeClientSettings.UModelExportRoot;
        _umodelSoundExport = L2InterludeClientSettings.UModelSoundExportRoot;
        _umodelSoundUnityExport = L2InterludeClientSettings.UModelSoundUnityExportRoot;
    }

    void SavePrefs()
    {
        L2InterludeClientSettings.ClientRoot = _clientRoot;
        L2InterludeClientSettings.UModelExportRoot = _umodelExport;
        L2InterludeClientSettings.UModelSoundExportRoot = _umodelSoundExport;
        L2InterludeClientSettings.UModelSoundUnityExportRoot = _umodelSoundUnityExport;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Interlude client → Unity batch (#476)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Requires local NCSoft client + UModel exports on disk. Never commit client binaries. " +
            "See Docs/INTERLUDE_TILE_BAKE.md for per-tile Shnok order and UModel prep.",
            MessageType.Info);

        _clientRoot = EditorGUILayout.TextField("Client root (Lineage II/)", _clientRoot);
        _umodelExport = EditorGUILayout.TextField("UModel mesh/texture export", _umodelExport);
        _umodelSoundExport = EditorGUILayout.TextField("UModel sound export", _umodelSoundExport);
        _umodelSoundUnityExport = EditorGUILayout.TextField("Ambient WAV staging folder", _umodelSoundUnityExport);

        if (GUILayout.Button("Save paths to EditorPrefs"))
        {
            SavePrefs();
        }

        EditorGUILayout.Space();
        _allowlist = EditorGUILayout.TextField("Allowlist (empty = all client maps)", _allowlist);
        _denylist = EditorGUILayout.TextField("Denylist", _denylist);

        EditorGUILayout.Space();
        _includeGlobalMaterials = EditorGUILayout.Toggle("Run Shnok/2 materials once per batch", _includeGlobalMaterials);
        _refreshManifestAfterRun = EditorGUILayout.Toggle("Refresh manifest after run", _refreshManifestAfterRun);
        DrawStepToggles();

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh manifest only"))
            {
                SavePrefs();
                RefreshManifestMenu();
            }

            if (GUILayout.Button("Run batch"))
            {
                SavePrefs();
                RunBatch();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Last run", EditorStyles.boldLabel);
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        for (int i = 0; i < _lastResults.Count; i++)
        {
            L2InterludeTileBakeResult r = _lastResults[i];
            EditorGUILayout.LabelField(r.tileId + (r.success ? " OK" : " FAIL"), EditorStyles.miniBoldLabel);
            for (int j = 0; j < r.log.Count; j++)
            {
                EditorGUILayout.LabelField("  • " + r.log[j], EditorStyles.miniLabel);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawStepToggles()
    {
        EditorGUILayout.LabelField("Steps", EditorStyles.boldLabel);
        _steps = SetFlag(_steps, L2InterludeBakeStep.ImportBrushTextures, "Brush import textures");
        _steps = SetFlag(_steps, L2InterludeBakeStep.ImportStaticMeshes, "1 StaticMeshes import");
        _steps = SetFlag(_steps, L2InterludeBakeStep.GenerateMaterials, "2 Materials (global)");
        _steps = SetFlag(_steps, L2InterludeBakeStep.GenerateStaticMeshScene, "3 StaticMeshes scene");
        _steps = SetFlag(_steps, L2InterludeBakeStep.GenerateTerrain, "4 Terrain");
        _steps = SetFlag(_steps, L2InterludeBakeStep.BuildBrushes, "5 Brushes");
        _steps = SetFlag(_steps, L2InterludeBakeStep.ImportAmbientSounds, "9 Ambient import");
        _steps = SetFlag(_steps, L2InterludeBakeStep.BuildAmbientSounds, "9.1 Ambient build");
        _steps = SetFlag(_steps, L2InterludeBakeStep.ConvertTerrainToMesh, "6 Terrain mesh");
        _steps = SetFlag(_steps, L2InterludeBakeStep.GenerateDecoLayerMesh, "7 Deco layer");
        _steps = SetFlag(_steps, L2InterludeBakeStep.BuildCameras, "8 Cameras");
    }

    static L2InterludeBakeStep SetFlag(L2InterludeBakeStep value, L2InterludeBakeStep flag, string label)
    {
        bool enabled = (value & flag) != 0;
        enabled = EditorGUILayout.ToggleLeft(label, enabled);
        return enabled ? (value | flag) : (value & ~flag);
    }

    void RunBatch()
    {
        if (!L2InterludeClientSettings.TryValidateUModelExportRoot(out string exportMsg))
        {
            EditorUtility.DisplayDialog("UModel export", exportMsg, "OK");
            return;
        }

        HashSet<string> tiles;
        if (L2InterludeClientSettings.TryValidateClientRoot(out _))
        {
            tiles = L2InterludeTileManifest.ScanClientTiles(L2InterludeClientSettings.ClientRoot);
        }
        else
        {
            tiles = L2InterludeTileManifest.ScanRepoTiles();
            if (tiles.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "Interlude batch",
                    "Set client root to discover maps/*.unr, or allowlist repo tiles.",
                    "OK");
                return;
            }
        }

        List<string> selected = L2InterludeTileManifest.FilterTiles(tiles.OrderBy(x => x), _allowlist, _denylist);
        if (selected.Count == 0)
        {
            EditorUtility.DisplayDialog("Interlude batch", "No tiles matched allow/deny filters.", "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog(
                "Run Interlude batch",
                $"Process {selected.Count} tile(s)?\nThis can take a long time.",
                "Run",
                "Cancel"))
        {
            return;
        }

        L2InterludeBakeStep steps = _steps;
        _lastResults = L2InterludeTileBakePipeline.RunTiles(selected, steps, _includeGlobalMaterials);

        if (_refreshManifestAfterRun)
        {
            L2InterludeTileManifest.WriteToRepo(L2InterludeTileManifest.Build());
        }

        Debug.Log($"Interlude batch finished: {selected.Count} tile(s).");
    }
}
#endif
