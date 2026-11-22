using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Interlude pools/sea are WaterVolume brushes, not StaticMeshes. Official bounds and the
/// map's water Shader/FinalBlend name live in Maps/XX_YY/Meta/watervolumes.json
/// (tools/interlude/WaterVolumeDump.java + tools/interlude-extract/rebuild_water_volumes.py).
/// This step adds child "L2WaterVolumes" (one quad per volume at its top Z) to
/// Maps/XX_YY/StaticMeshes.prefab and assigns the official Unity material
/// (typically FX_E_T/Shader/WaterShader01.mat). Missing json, prefab, or official
/// material logs an error — no silent shared-tint fallback.
/// UE -> Unity: (y, z, x) / 52.5, same as L2MapStaticMeshBuilder. Idempotent (child is rebuilt each run).
///
/// Unity -batchmode -quit -projectPath l2-unity -executeMethod L2InterludeWaterVolumes.RunCli
/// (env FINEAGE_INTERLUDE_TILES=21_23,22_23,... ; do not pass -nographics; close other Unity first;
///  not ~/Projects/Fineage).
/// </summary>
public static class L2InterludeWaterVolumes
{
    const string ChildName = "L2WaterVolumes";
    const float UeToUnity = 1f / 52.5f;
    static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

    [MenuItem("Shnok/0.4. [Interlude] Build water surfaces from WaterVolumes (all tiles)")]
    public static void Menu() { Debug.Log("[Water] " + Run(null)); }

    public static void RunCli()
    {
        string raw = Environment.GetEnvironmentVariable("FINEAGE_INTERLUDE_TILES");
        IEnumerable<string> tiles = string.IsNullOrWhiteSpace(raw) ? null
            : raw.Split(',', StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("[Water] " + Run(tiles));
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(0);
    }

    /// <param name="tiles">null = every tile that has Maps/&lt;tile&gt;/Meta/watervolumes.json</param>
    public static string Run(IEnumerable<string> tiles)
    {
        var list = new List<string>();
        if (tiles != null) foreach (var t in tiles) list.Add(t.Trim());
        else
        {
            string maps = Path.Combine(Application.dataPath, "Resources/Data/Maps");
            if (Directory.Exists(maps))
                foreach (var d in Directory.GetDirectories(maps))
                    if (File.Exists(Path.Combine(d, "Meta", "watervolumes.json")))
                        list.Add(Path.GetFileName(d));
        }

        int quads = 0, prefabs = 0;
        var notes = new List<string>();
        foreach (var tile in list)
        {
            string json = Path.Combine(Application.dataPath, "Resources/Data/Maps", tile, "Meta", "watervolumes.json");
            if (!File.Exists(json))
                json = Path.Combine(RepoRoot, ".work", "interlude-extract", tile, "watervolumes.json");
            string prefab = "Assets/Resources/Data/Maps/" + tile + "/StaticMeshes.prefab";
            if (!File.Exists(json))
            {
                Debug.LogError("[Water] " + tile + ": missing watervolumes.json (looked in Maps/" + tile + "/Meta and .work/interlude-extract)");
                notes.Add(tile + ": no json");
                continue;
            }
            if (!File.Exists(prefab))
            {
                Debug.LogError("[Water] " + tile + ": missing StaticMeshes.prefab; run the tile bake first, then rebuild water");
                notes.Add(tile + ": no StaticMeshes.prefab");
                continue;
            }

            string text = File.ReadAllText(json);
            var boxes = Parse(text);
            var mat = ResolveOfficialMaterial(tile, text);
            if (mat == null)
            {
                notes.Add(tile + ": no official material");
                continue;
            }

            var verts = new List<Vector3>(); var uvs = new List<Vector2>(); var tris = new List<int>();
            foreach (var b in boxes)
            {
                Vector3 mn = b.Item1, mx = b.Item2;
                if (mx.x - mn.x < 1f || mx.y - mn.y < 1f) continue;
                float y = mx.z * UeToUnity;
                int i0 = verts.Count;
                foreach (var c in new[] { new Vector2(mn.x, mn.y), new Vector2(mn.x, mx.y), new Vector2(mx.x, mx.y), new Vector2(mx.x, mn.y) })
                {
                    var p = new Vector3(c.y * UeToUnity, y, c.x * UeToUnity);
                    verts.Add(p);
                    uvs.Add(new Vector2(p.x, p.z) / 6f);
                }
                tris.AddRange(new[] { i0, i0 + 1, i0 + 2, i0, i0 + 2, i0 + 3 });
                quads++;
                notes.Add(tile + "/" + b.Item3 + " y=" + y.ToString("F2", CultureInfo.InvariantCulture));
            }

            string meshPath = "Assets/Resources/Data/Maps/" + tile + "/" + tile + "_Water.asset";
            Mesh mesh = null;
            if (verts.Count > 0)
            {
                mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                if (mesh == null) { mesh = new Mesh(); AssetDatabase.CreateAsset(mesh, meshPath); }
                mesh.Clear();
                mesh.name = tile + "_Water";
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.SetVertices(verts); mesh.SetUVs(0, uvs); mesh.SetTriangles(tris, 0);
                mesh.RecalculateNormals(); mesh.RecalculateBounds(); mesh.RecalculateTangents();
                EditorUtility.SetDirty(mesh);
                AssetDatabase.SaveAssets();
            }

            var root = PrefabUtility.LoadPrefabContents(prefab);
            try
            {
                var old = root.transform.Find(ChildName);
                if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
                if (mesh != null)
                {
                    var go = new GameObject(ChildName);
                    go.transform.SetParent(root.transform, false);
                    go.transform.position = Vector3.zero; go.transform.rotation = Quaternion.identity;
                    var ls = root.transform.lossyScale;
                    go.transform.localScale = new Vector3(1f / ls.x, 1f / ls.y, 1f / ls.z);
                    int wl = LayerMask.NameToLayer("Water");
                    if (wl >= 0) go.layer = wl;
                    go.AddComponent<MeshFilter>().sharedMesh = mesh;
                    var mr = go.AddComponent<MeshRenderer>();
                    mr.sharedMaterial = mat;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                PrefabUtility.SaveAsPrefabAsset(root, prefab);
                prefabs++;
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        return "water surfaces: " + quads + " quad(s) in " + prefabs + " prefab(s)" + (notes.Count > 0 ? " [" + string.Join("; ", notes) + "]" : "");
    }

    static List<(Vector3, Vector3, string)> Parse(string s)
    {
        var r = new List<(Vector3, Vector3, string)>();
        var rx = new Regex("\"name\"\\s*:\\s*\"([^\"]+)\"\\s*,\\s*\"min\"\\s*:\\s*\\[([^\\]]+)\\]\\s*,\\s*\"max\"\\s*:\\s*\\[([^\\]]+)\\]", RegexOptions.Singleline);
        foreach (Match m in rx.Matches(s)) r.Add((V(m.Groups[2].Value), V(m.Groups[3].Value), m.Groups[1].Value));
        return r;
    }

    static Vector3 V(string s)
    {
        var p = s.Split(',');
        return new Vector3(float.Parse(p[0], CultureInfo.InvariantCulture), float.Parse(p[1], CultureInfo.InvariantCulture), float.Parse(p[2], CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Resolves the official Shader/FinalBlend named in watervolumes.json. Does not invent
    /// L2WaterSurface.mat or a hard-coded tint when the official material is missing.
    /// </summary>
    static Material ResolveOfficialMaterial(string tile, string json)
    {
        var names = new List<string>();
        var one = Regex.Match(json, "\"material\"\\s*:\\s*\"([^\"]+)\"");
        if (one.Success) names.Add(one.Groups[1].Value);
        var many = Regex.Match(json, "\"materials\"\\s*:\\s*\\[([^\\]]*)\\]");
        if (many.Success)
            foreach (Match m in Regex.Matches(many.Groups[1].Value, "\"([^\"]+)\""))
                if (!names.Contains(m.Groups[1].Value)) names.Add(m.Groups[1].Value);

        if (names.Count == 0)
        {
            Debug.LogError("[Water] " + tile + ": watervolumes.json has no official material name");
            return null;
        }

        foreach (var name in names)
        {
            var mat = LoadUeMaterial(name);
            if (mat != null) return mat;
        }
        Debug.LogError("[Water] " + tile + ": official material not in the project: " + string.Join(", ", names));
        return null;
    }

    static Material LoadUeMaterial(string ueName)
    {
        if (string.IsNullOrEmpty(ueName)) return null;
        var parts = ueName.Split('.');
        string file = parts[parts.Length - 1];
        string pkg = parts[0];
        string[] folders =
        {
            "Assets/Resources/Data/Textures/" + pkg + "/Shader/" + file + ".mat",
            "Assets/Resources/Data/Textures/" + pkg + "/Materials/" + file + ".mat",
            "Assets/Resources/Data/Textures/" + pkg + "/" + file + ".mat",
        };
        foreach (var path in folders)
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m != null) return m;
        }
        foreach (var g in AssetDatabase.FindAssets(file + " t:Material", new[] { "Assets/Resources/Data/Textures" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            if (Path.GetFileNameWithoutExtension(path) == file)
                return AssetDatabase.LoadAssetAtPath<Material>(path);
        }
        return null;
    }
}
