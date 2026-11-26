using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Interlude pools/sea are WaterVolume brushes. Official brush geometry and the
/// map's water Shader/FinalBlend import come from the #1670/#1728 dumps
/// (l2-unity/docs/interlude-ref/world/maps/&lt;tile&gt;.unr.json.gz). This step
/// rebuilds child "L2WaterVolumes" (top faces of each volume brush) on
/// Maps/XX_YY/StaticMeshes.prefab and assigns the official Unity material
/// (typically FX_E_T/Shader/WaterShader01.mat). Missing dump, volumes, brush
/// polys, or official material logs an error — no Meta json and no silent
/// shared-tint fallback.
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
    const string DumpRel = "docs/interlude-ref/world/maps";
    static readonly HashSet<string> ShaderClasses = new HashSet<string> { "Shader", "FinalBlend", "TexPanner", "TexEnvMap", "Combiner" };
    static readonly HashSet<string> ShaderNames = new HashSet<string>
    {
        "WaterShader01", "WaterShaderPan01", "WaterFinal", "ssq_water_01sh"
    };

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

    /// <param name="tiles">null = every Maps/&lt;tile&gt; folder in the project.</param>
    public static string Run(IEnumerable<string> tiles)
    {
        var list = new List<string>();
        if (tiles != null) foreach (var t in tiles) list.Add(t.Trim());
        else
        {
            string maps = Path.Combine(Application.dataPath, "Resources/Data/Maps");
            if (Directory.Exists(maps))
                foreach (var d in Directory.GetDirectories(maps))
                {
                    string name = Path.GetFileName(d);
                    if (name.Length > 0 && char.IsDigit(name[0]) && name.Contains("_"))
                        list.Add(name);
                }
        }

        int quads = 0, prefabs = 0;
        var notes = new List<string>();
        foreach (var tile in list)
        {
            string dumpPath = FindDump(tile);
            if (dumpPath == null)
            {
                Debug.LogError("[Water] " + tile + ": official #1670 dump not found (looked in " + DumpRel + "/" + tile + ".unr.json.gz)");
                notes.Add(tile + ": no dump");
                continue;
            }

            JObject dump;
            try { dump = LoadDump(dumpPath); }
            catch (Exception e)
            {
                Debug.LogError("[Water] " + tile + ": failed to read official dump " + dumpPath + ": " + e.Message);
                notes.Add(tile + ": dump parse error");
                continue;
            }

            var volumes = new List<JObject>();
            foreach (var token in dump["exports"] ?? new JArray())
            {
                if (token is JObject exp && (string)exp["class"] == "WaterVolume")
                    volumes.Add(exp);
            }
            if (volumes.Count == 0)
            {
                Debug.LogError("[Water] " + tile + ": official dump has no WaterVolume actors");
                notes.Add(tile + ": no WaterVolume in dump");
                continue;
            }

            var shaderNames = OfficialWaterShaders(dump);
            var mat = ResolveOfficialMaterial(tile, shaderNames);
            if (mat == null)
            {
                notes.Add(tile + ": no official material");
                continue;
            }

            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            foreach (var exp in volumes)
            {
                string name = (string)exp["name"] ?? "WaterVolume";
                int added = AppendVolumeSurface(tile, name, exp, verts, uvs, tris);
                if (added > 0)
                {
                    quads += added;
                    float y = verts[verts.Count - 1].y;
                    notes.Add(tile + "/" + name + " y=" + y.ToString("F2", CultureInfo.InvariantCulture));
                }
            }

            string prefab = "Assets/Resources/Data/Maps/" + tile + "/StaticMeshes.prefab";
            if (!File.Exists(prefab))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(Application.dataPath, "Resources/Data/Maps", tile, "StaticMeshes.prefab")));
                var tmp = new GameObject("StaticMeshes");
                PrefabUtility.SaveAsPrefabAsset(tmp, prefab);
                UnityEngine.Object.DestroyImmediate(tmp);
                Debug.Log("[Water] " + tile + ": created StaticMeshes.prefab to hold official WaterVolume surfaces");
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
                    if (Mathf.Abs(ls.x) > 0.0001f && Mathf.Abs(ls.y) > 0.0001f && Mathf.Abs(ls.z) > 0.0001f)
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
        return "water surfaces: " + quads + " top-face(s) in " + prefabs + " prefab(s)" + (notes.Count > 0 ? " [" + string.Join("; ", notes) + "]" : "");
    }

    static string FindDump(string tile)
    {
        string root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string gz = Path.Combine(root, DumpRel, tile + ".unr.json.gz");
        if (File.Exists(gz)) return gz;
        string json = Path.Combine(root, DumpRel, tile + ".unr.json");
        return File.Exists(json) ? json : null;
    }

    static JObject LoadDump(string path)
    {
        using (var fs = File.OpenRead(path))
        using (Stream stream = path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
                   ? (Stream)new GZipStream(fs, CompressionMode.Decompress, leaveOpen: true)
                   : fs)
        using (var sr = new StreamReader(stream))
        using (var reader = new JsonTextReader(sr))
            return (JObject)JToken.Load(reader);
    }

    static List<string> OfficialWaterShaders(JObject dump)
    {
        var found = new List<string>();
        foreach (var token in dump["imports"] ?? new JArray())
        {
            if (token is not JObject im) continue;
            string name = (string)im["object_name"] ?? "";
            string cls = (string)im["class_name"] ?? "";
            string pkg = (string)im["package"] ?? "";
            if (!ShaderClasses.Contains(cls)) continue;
            if (!ShaderNames.Contains(name) && name.IndexOf("Water", StringComparison.OrdinalIgnoreCase) < 0)
                continue;
            string leaf = pkg;
            int colon = leaf.LastIndexOf(':');
            if (colon >= 0) leaf = leaf.Substring(colon + 1);
            string full;
            if (leaf == "WaterSurfaceShaderSet") full = "FX_E_T.WaterSurfaceShaderSet." + name;
            else if (leaf == "WaterSurfaceSet") full = "FX_E_T.WaterSurfaceSet." + name;
            else if (leaf == "FX_E_T") full = "FX_E_T." + name;
            else full = string.IsNullOrEmpty(leaf) ? name : leaf + "." + name;
            if (!found.Contains(full)) found.Add(full);
        }
        found.Sort((a, b) =>
        {
            int ka = a.EndsWith("WaterShader01", StringComparison.Ordinal) ? 0 : 1;
            int kb = b.EndsWith("WaterShader01", StringComparison.Ordinal) ? 0 : 1;
            int c = ka.CompareTo(kb);
            return c != 0 ? c : string.CompareOrdinal(a, b);
        });
        return found;
    }

    static Material ResolveOfficialMaterial(string tile, List<string> names)
    {
        if (names == null || names.Count == 0)
        {
            Debug.LogError("[Water] " + tile + ": official dump has WaterVolume actors but no water Shader/FinalBlend import");
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

    /// <returns>Number of top faces added for this volume (0 if skipped after LogError).</returns>
    static int AppendVolumeSurface(string tile, string name, JObject exp, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
    {
        var vol = exp["volume"] as JObject;
        var model = vol?["model"] as JObject;
        var polys = model?["polys"] as JArray;
        if (polys == null || polys.Count == 0)
        {
            Debug.LogError("[Water] " + tile + "/" + name + ": WaterVolume has no brush polys in the official dump");
            return 0;
        }

        Vector3 location = Vec(vol["location"]);
        Vector3 prepivot = Vec(vol["prepivot"]);
        Vector3 scale = Vec(vol["scale"], Vector3.one);
        Vector3 draw = Vec(vol["draw_scale_3d"], Vector3.one);
        Vector3 rot = Rot(vol["rotation"]);

        int faces = 0;
        foreach (var token in polys)
        {
            if (token is not JObject poly) continue;
            var rawVerts = poly["vertices"] as JArray;
            if (rawVerts == null || rawVerts.Count < 3)
            {
                Debug.LogError("[Water] " + tile + "/" + name + ": brush poly has fewer than 3 vertices");
                continue;
            }
            var world = new List<Vector3>(rawVerts.Count);
            for (int i = 0; i < rawVerts.Count; i++)
                world.Add(OfficialBrushTransform.ToWorld(Vec(rawVerts[i]), location, prepivot, scale, draw, rot));

            Vector3 nrm = Vec(poly["normal"]);
            nrm = OfficialBrushTransform.RotateNormalUe(nrm, scale, draw, rot);
            if (nrm.sqrMagnitude < 0.0001f && world.Count >= 3)
                nrm = Vector3.Cross(world[1] - world[0], world[2] - world[0]).normalized;
            if (!OfficialBrushTransform.IsTopFace(nrm))
                continue;

            int i0 = verts.Count;
            if (!OfficialBrushTransform.AppendPolygon(ToUnity(world), verts, tris))
                continue;
            for (int i = i0; i < verts.Count; i++)
                uvs.Add(new Vector2(verts[i].x, verts[i].z) / 6f);
            faces++;
        }

        if (faces == 0)
            Debug.LogError("[Water] " + tile + "/" + name + ": WaterVolume brush has no upward top face in the official dump");
        return faces;
    }

    static List<Vector3> ToUnity(List<Vector3> ue)
    {
        var u = new List<Vector3>(ue.Count);
        for (int i = 0; i < ue.Count; i++)
        {
            Vector3 p = ue[i];
            u.Add(new Vector3(p.y * UeToUnity, p.z * UeToUnity, p.x * UeToUnity));
        }
        return u;
    }

    static Vector3 Vec(JToken token, Vector3 fallback = default)
    {
        if (token is not JObject o) return fallback;
        return new Vector3(
            (float)(o["x"] ?? fallback.x),
            (float)(o["y"] ?? fallback.y),
            (float)(o["z"] ?? fallback.z));
    }

    static Vector3 Rot(JToken token)
    {
        if (token is not JObject o) return Vector3.zero;
        return new Vector3(
            (float)(o["pitch"] ?? 0),
            (float)(o["yaw"] ?? 0),
            (float)(o["roll"] ?? 0));
    }
}
