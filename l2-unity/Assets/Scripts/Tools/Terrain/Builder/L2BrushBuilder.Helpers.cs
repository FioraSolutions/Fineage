#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class L2BrushBuilder
{
    public static GameObject BuildBrushesFromT3d(string t3dAbsolutePath) {
        Brush[] brushes = L2T3DInfoParser.ParseBrushInfo(t3dAbsolutePath).ToArray();
        // <tile>/Meta/<tile>.t3d -> Assets/.../Maps/<tile>
        string tileAbs = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetFullPath(t3dAbsolutePath))).Replace('\\', '/');
        string dataAbs = Path.GetFullPath(Application.dataPath).Replace('\\', '/');
        if (tileAbs.StartsWith(dataAbs))
            return BuildMerged(brushes, "Assets" + tileAbs.Substring(dataAbs.Length));
        return Build(brushes);
    }

    public static GameObject BuildMerged(Brush[] brushes, string tileAssetFolder) {
        var matCache = new Dictionary<string, Material>();
        var subs = new Dictionary<Material, List<int>>();
        var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>();
        int polyN = 0, skippedBrush = 0;
        foreach (Brush b in brushes) {
            if (b.position == Vector3.zero || b.model?.poly?.polyData == null) { skippedBrush++; continue; }
            if (b.polyFlags != null && (b.polyFlags.Contains("PF_Invisible") || b.polyFlags.Contains("PF_NotSolid"))) { skippedBrush++; continue; }
            Vector3 basePos = b.position - b.prePivot;
            bool huge = false;
            foreach (var pd in b.model.poly.polyData) foreach (var v in pd.vertices) if (Mathf.Abs(v.x) > 60000 || Mathf.Abs(v.y) > 60000 || Mathf.Abs(v.z) > 60000) huge = true;
            if (huge) { skippedBrush++; continue; } // world-bounds subtract cube etc.
            foreach (PolyData pd in b.model.poly.polyData) {
                if (pd.vertices == null || pd.vertices.Length < 3) continue;
                var fl = pd.polyFlags ?? new string[0];
                if (fl.Contains("PF_Invisible") || fl.Contains("PF_NotSolid") || fl.Contains("PF_Portal")) continue;
                string tex = pd.texture;
                if (string.IsNullOrEmpty(tex) || tex.StartsWith("L2_Skies") || tex.StartsWith("L2_ETC_Textures") || tex.StartsWith("Engine.")
                    || tex.IndexOf("Water", System.StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (!matCache.TryGetValue(tex, out var mat)) { mat = FindMaterial(tex); matCache[tex] = mat; }
                float tw = 256f, th = 256f;
                var mt = mat != null && mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : mat != null ? mat.mainTexture : null;
                if (mt != null) { tw = mt.width; th = mt.height; }
                int start = verts.Count, n = pd.vertices.Length;
                var w = new Vector3[n];
                for (int i = 0; i < n; i++) w[i] = VectorUtils.ConvertPosToUnity(basePos + pd.vertices[i]) * L2TerrainGeneratorTool.MAP_SCALE;
                Vector3 nrm = Vector3.Cross(w[1] - w[0], w[2] - w[0]);
                for (int i = 3; nrm.sqrMagnitude < 1e-10f && i < n; i++) nrm = Vector3.Cross(w[1] - w[0], w[i] - w[0]);
                if (nrm.sqrMagnitude < 1e-10f) continue;
                nrm.Normalize();
                if (!subs.TryGetValue(mat, out var tris)) subs[mat] = tris = new List<int>();
                for (int side = 0; side < 2; side++) {
                    int s0 = verts.Count;
                    for (int i = 0; i < n; i++) {
                        Vector3 d = pd.vertices[i] - pd.origin;
                        verts.Add(w[i]);
                        norms.Add(side == 0 ? nrm : -nrm);
                        uvs.Add(new Vector2(Vector3.Dot(d, pd.textureU) / tw, -Vector3.Dot(d, pd.textureV) / th));
                    }
                    for (int i = 1; i < n - 1; i++) {
                        if (side == 0) { tris.Add(s0); tris.Add(s0 + i); tris.Add(s0 + i + 1); }
                        else { tris.Add(s0); tris.Add(s0 + i + 1); tris.Add(s0 + i); }
                    }
                }
                polyN++;
            }
        }
        GameObject root = new GameObject("Brushes");
        if (polyN == 0) { Debug.Log("[Brushes] no BSP polys (" + brushes.Length + " brushes, skipped " + skippedBrush + ")"); return root; }

        var mesh = new Mesh { name = Path.GetFileName(tileAssetFolder) + "_BSP", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.SetVertices(verts); mesh.SetNormals(norms); mesh.SetUVs(0, uvs);
        var mats = subs.Keys.ToList();
        mesh.subMeshCount = mats.Count;
        for (int i = 0; i < mats.Count; i++) mesh.SetTriangles(subs[mats[i]], i);
        mesh.RecalculateBounds(); mesh.RecalculateTangents();
        string meshPath = tileAssetFolder + "/Brushes_Mesh.asset";
        if (AssetDatabase.LoadAssetAtPath<Mesh>(meshPath) != null) AssetDatabase.DeleteAsset(meshPath);
        AssetDatabase.CreateAsset(mesh, meshPath);
        AssetDatabase.SaveAssets();

        var go = new GameObject("BSP");
        go.transform.SetParent(root.transform, false);
        int layer = LayerMask.NameToLayer("Brush"); if (layer < 0) layer = LayerMask.NameToLayer("StaticMesh");
        if (layer >= 0) { go.layer = layer; root.layer = layer; }
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
        go.AddComponent<MeshCollider>().sharedMesh = mesh;
        go.isStatic = true;
        Debug.Log("[Brushes] BSP polys " + polyN + ", materials " + mats.Count + ", brushes " + brushes.Length + " (skipped " + skippedBrush + ") -> " + meshPath);
        return root;
    }
}
#endif