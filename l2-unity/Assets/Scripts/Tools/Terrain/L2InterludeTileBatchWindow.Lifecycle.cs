#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class L2InterludeTileBatchWindow : EditorWindow
{
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
}
#endif
