using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Interlude pools/sea (Mother Tree basin in 21_19, Giran sea, ...) are not StaticMeshes: L2 draws them from
/// WaterVolume brushes, which the StaticMesh bake never exported -> dry basins. Phase A now writes
/// .work/interlude-extract/XX_YY/watervolumes.json (tools/interlude/WaterVolumeDump.java, brush bounds in UE units);
/// this step adds child "L2WaterVolumes" (one quad per volume at its top Z) to Maps/XX_YY/StaticMeshes.prefab.
/// UE -> Unity: (y, z, x) / 52.5, same as L2MapStaticMeshBuilder. Idempotent (child is rebuilt each run).
/// </summary>
public static class L2InterludeWaterVolumes
{
    const string ChildName = "L2WaterVolumes";
    const float UeToUnity = 1f / 52.5f;
    static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

    [MenuItem("Shnok/0.4. [Interlude] Build water surfaces from WaterVolumes (all tiles)")]
    public static void Menu() { Debug.Log("[Water] " + Run(null)); }

    /// <param name="tiles">null = every tile that has watervolumes.json</param>
    public static string Run(IEnumerable<string> tiles)
    {
        string extract = Path.Combine(RepoRoot, ".work", "interlude-extract");
        var list = new List<string>();
        if (tiles != null) foreach (var t in tiles) list.Add(t.Trim());
        else if (Directory.Exists(extract))
            foreach (var d in Directory.GetDirectories(extract))
                if (File.Exists(Path.Combine(d, "watervolumes.json"))) list.Add(Path.GetFileName(d));

        var mat = WaterMaterial();
        int quads = 0, prefabs = 0;
        var notes = new List<string>();
        foreach (var tile in list)
        {
            string json = Path.Combine(extract, tile, "watervolumes.json");
            string prefab = "Assets/Resources/Data/Maps/" + tile + "/StaticMeshes.prefab";
            if (!File.Exists(json) || !File.Exists(prefab)) { notes.Add(tile + ": no json/prefab"); continue; }
            var boxes = Parse(File.ReadAllText(json));

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
                    uvs.Add(new Vector2(p.x, p.z) / 6f); // ~6 m texture tile
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
        var rx = new Regex("\"name\"\\s*:\\s*\"([^\"]+)\"\\s*,\\s*\"min\"\\s*:\\s*\\[([^\\]]+)\\]\\s*,\\s*\"max\"\\s*:\\s*\\[([^\\]]+)\\]");
        foreach (Match m in rx.Matches(s)) r.Add((V(m.Groups[2].Value), V(m.Groups[3].Value), m.Groups[1].Value));
        return r;
    }

    static Vector3 V(string s)
    {
        var p = s.Split(',');
        return new Vector3(float.Parse(p[0], CultureInfo.InvariantCulture), float.Parse(p[1], CultureInfo.InvariantCulture), float.Parse(p[2], CultureInfo.InvariantCulture));
    }

    /// <summary>Shared transparent URP water material; seeded from Oren water texture when available.</summary>
    static Material WaterMaterial()
    {
        const string path = "Assets/Resources/Data/Maps/L2WaterSurface.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "L2WaterSurface" };
        Texture2D t = null;
        foreach (var g in AssetDatabase.FindAssets("Water02 t:Texture2D", new[] { "Assets/Resources/Data/Textures" }))
        { t = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(g)); if (t != null) break; }
        if (t != null) { m.SetTexture("_BaseMap", t); m.SetTexture("_MainTex", t); }
        m.SetFloat("_Surface", 1f); m.SetFloat("_Blend", 0f);
        m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetFloat("_SrcBlendAlpha", (float)UnityEngine.Rendering.BlendMode.One);
        m.SetFloat("_DstBlendAlpha", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetFloat("_ZWrite", 0f); m.SetFloat("_AlphaClip", 0f); m.SetFloat("_Cull", 0f);
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.SetOverrideTag("RenderType", "Transparent");
        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        var c = new Color(0.45f, 0.65f, 0.75f, 0.65f);
        m.SetColor("_BaseColor", c); m.SetColor("_Color", c);
        m.SetFloat("_Smoothness", 0.9f);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }
}
