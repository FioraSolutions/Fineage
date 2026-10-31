#if UNITY_EDITOR
using System;
using UnityEditor;
/// Unity -batchmode -nographics -executeMethod L2TerrainMeshCli.RunCli  (env FINEAGE_INTERLUDE_TILES=21_19)
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
