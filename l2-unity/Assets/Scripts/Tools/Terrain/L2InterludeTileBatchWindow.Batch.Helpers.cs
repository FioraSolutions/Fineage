#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class L2InterludeTileBatchWindow : EditorWindow
{
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
