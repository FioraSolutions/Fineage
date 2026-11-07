#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// While the Editor is already open (batchmode cannot attach), drop a flag file to bake
/// missing Interlude tile scenes into Build Settings. Watched on editor update.
/// Flag: Assets/Editor/FineagePendingTileScenes.txt — one line of comma-separated XX_YY tiles.
/// </summary>
[InitializeOnLoad]
static class FineagePendingTileSceneBuild
{
    const string FlagPath = "Assets/Editor/FineagePendingTileScenes.txt";
    static bool _running;

    static FineagePendingTileSceneBuild()
    {
        EditorApplication.delayCall += TryConsume;
    }

    static void TryConsume()
    {
        if (_running || EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        if (!File.Exists(FlagPath))
            return;

        string raw = File.ReadAllText(FlagPath).Trim();
        File.Delete(FlagPath);
        AssetDatabase.Refresh();
        if (string.IsNullOrEmpty(raw))
            return;

        _running = true;
        try
        {
            bool ok = true;
            foreach (string tile in raw.Split(',', System.StringSplitOptions.RemoveEmptyEntries))
            {
                string t = tile.Trim();
                if (t.Length == 0)
                    continue;
                ok &= L2InterludeTileSceneCli.BuildScene(t);
            }
            Debug.Log(ok
                ? "[FineagePendingTileSceneBuild] tile scenes ready"
                : "[FineagePendingTileSceneBuild] one or more tiles failed — see console");
        }
        finally
        {
            _running = false;
        }
    }
}
#endif

