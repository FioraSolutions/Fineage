#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class L2InterludeTileBatchWindow : EditorWindow
{
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
}
#endif
