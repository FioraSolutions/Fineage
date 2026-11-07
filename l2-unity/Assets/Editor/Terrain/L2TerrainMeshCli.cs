#if UNITY_EDITOR
using System;
using UnityEditor;
/// Unity -batchmode (NO -nographics: texture arrays need GPU Blit) -executeMethod L2TerrainMeshCli.RunCli  (env FINEAGE_INTERLUDE_TILES=21_19)
/// Runs only the ConvertTerrainToMesh step (heightmap mesh + material + safenet) per tile.
public static class L2TerrainMeshCli {
    public static void RunCli() {
        string raw = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILES") ?? "21_19";
        bool ok = true;
        foreach (var t in raw.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
            ok &= TerrainConverter.ConvertTerrainToMeshForMap(t.Trim());
        }
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(ok ? 0 : 1);
    }
}
#endif
#if UNITY_EDITOR
public static class L2TerrainMeshMenu {
    [UnityEditor.MenuItem("Shnok/0.3. [Interlude] Re-convert terrain mesh + textures (21_19)")]
    static void Reconvert21_19() {
        TerrainConverter.ConvertTerrainToMeshForMap("21_19");
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEngine.Debug.Log("[L2TerrainMeshMenu] 21_19 terrain re-converted");
    }
}
#endif
