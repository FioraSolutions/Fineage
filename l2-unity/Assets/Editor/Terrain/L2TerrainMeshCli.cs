#if UNITY_EDITOR
using System;
using UnityEditor;
/// Unity -batchmode (NO -nographics when also writing terrain meshes: texture arrays need GPU Blit)
/// -executeMethod L2TerrainMeshCli.RunCli
/// FINEAGE_INTERLUDE_TILES=21_19
/// FINEAGE_DECO_LAYER=1 writes XX_YY_Deco.bytes only, unless FINEAGE_TERRAIN_MESH=1 as well.
/// Deco-only reads density PNGs on the CPU and does not need the terrain prefab.
public static class L2TerrainMeshCli {
    public static void RunCli() {
        string raw = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILES") ?? "21_19";
        bool deco = Environment.GetEnvironmentVariable("FINEAGE_DECO_LAYER") == "1";
        string meshEnv = Environment.GetEnvironmentVariable("FINEAGE_TERRAIN_MESH");
        bool terrain = deco ? meshEnv == "1" : true;
        bool ok = true;
        foreach (var t in raw.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
            try {
                ok &= TerrainConverter.ConvertTerrainToMeshForMap(t.Trim(), terrain, deco);
            } catch (Exception e) {
                UnityEngine.Debug.LogError("[L2TerrainMeshCli] " + t.Trim() + " failed: " + e);
                ok = false;
            }
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
