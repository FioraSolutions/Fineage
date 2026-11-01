using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Interlude-baked tiles' StaticMeshes.prefab had ZERO MeshColliders (legacy tiles 16_24/16_25 have one per
/// MeshFilter on layer StaticMesh). The terrain has holes under HEV structures (Mother Tree basin), so the player
/// fell through rims/walkways. Adds a non-convex MeshCollider to every StaticMesh MeshFilter (except water/fx
/// surfaces) and puts it on the StaticMesh layer used by GroundSnapHelper. Idempotent.
/// </summary>
public static class L2InterludeStaticMeshColliders
{
    static bool NoCollide(string n)
    {
        n = n.ToLowerInvariant();
        return n.Contains("water") || n.Contains("waterfall") || n.Contains("_fx") || n.Contains("light") && n.Contains("beam");
    }

    [MenuItem("Shnok/0.3. [Interlude] Add StaticMesh colliders (all tiles)")]
    public static void Menu() { Debug.Log("[Colliders] " + Run(null)); }

    /// <param name="tiles">null = all Maps/*/StaticMeshes.prefab</param>
    public static string Run(IEnumerable<string> tiles)
    {
        int layer = LayerMask.NameToLayer("StaticMesh");
        var paths = new List<string>();
        if (tiles == null)
        {
            foreach (var d in Directory.GetDirectories("Assets/Resources/Data/Maps"))
                paths.Add(Path.Combine(d, "StaticMeshes.prefab").Replace('\\', '/'));
        }
        else foreach (var t in tiles) paths.Add("Assets/Resources/Data/Maps/" + t.Trim() + "/StaticMeshes.prefab");

        int added = 0, prefabs = 0;
        foreach (var p in paths)
        {
            if (!File.Exists(p)) continue;
            var root = PrefabUtility.LoadPrefabContents(p);
            int before = added;
            try
            {
                foreach (var mf in root.GetComponentsInChildren<MeshFilter>(true))
                {
                    if (mf.sharedMesh == null || NoCollide(mf.gameObject.name)) continue;
                    if (layer >= 0 && mf.gameObject.layer == 0) mf.gameObject.layer = layer;
                    var mc = mf.GetComponent<MeshCollider>();
                    if (mc == null) { mc = mf.gameObject.AddComponent<MeshCollider>(); added++; }
                    if (mc.sharedMesh == null) mc.sharedMesh = mf.sharedMesh;
                }
                if (added != before) { PrefabUtility.SaveAsPrefabAsset(root, p); prefabs++; }
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        return "StaticMesh colliders added: " + added + " in " + prefabs + " prefab(s)";
    }
}
