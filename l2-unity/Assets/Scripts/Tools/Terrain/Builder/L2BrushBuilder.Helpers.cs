#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using System;

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
        int polyN = 0, skippedBrush = 0, punched = 0;
        List<BspPortal> portals = CollectPortalCaps(brushes);
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
                List<Vector3[]> pieces = PortalPieces(b, pd, basePos, tex, portals);
                if (pieces == null)
                    continue;
                if (pieces.Count != 1)
                    punched++;
                for (int piece = 0; piece < pieces.Count; piece++)
                {
                    Vector3[] local = pieces[piece];
                    if (local == null || local.Length < 3)
                        continue;
                    int n = local.Length;
                    var w = new Vector3[n];
                    for (int i = 0; i < n; i++) w[i] = VectorUtils.ConvertPosToUnity(basePos + local[i]) * L2TerrainGeneratorTool.MAP_SCALE;
                    Vector3 nrm = Vector3.Cross(w[1] - w[0], w[2] - w[0]);
                    for (int i = 3; nrm.sqrMagnitude < 1e-10f && i < n; i++) nrm = Vector3.Cross(w[1] - w[0], w[i] - w[0]);
                    if (nrm.sqrMagnitude < 1e-10f) continue;
                    nrm.Normalize();
                    if (!subs.TryGetValue(mat, out var tris)) subs[mat] = tris = new List<int>();
                    for (int side = 0; side < 2; side++) {
                        int s0 = verts.Count;
                        for (int i = 0; i < n; i++) {
                            Vector3 d = local[i] - pd.origin;
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
        Debug.Log("[Brushes] BSP polys " + polyN + ", portal cuts " + punched + ", materials " + mats.Count + ", brushes " + brushes.Length + " (skipped " + skippedBrush + ") -> " + meshPath);
        return root;
    }

    struct BspPortal
    {
        public string Brush;
        public int Axis;
        public float Plane;
        public int A;
        public int B;
        public float A0, A1, B0, B1;
    }

    static float AxisOf(Vector3 v, int axis)
    {
        if (axis == 0) return v.x;
        if (axis == 1) return v.y;
        return v.z;
    }

    static Vector3 WithAxis(int axis, float plane, int a, float av, int b, float bv)
    {
        Vector3 v = Vector3.zero;
        if (axis == 0) v.x = plane;
        else if (axis == 1) v.y = plane;
        else v.z = plane;
        if (a == 0) v.x = av;
        else if (a == 1) v.y = av;
        else v.z = av;
        if (b == 0) v.x = bv;
        else if (b == 1) v.y = bv;
        else v.z = bv;
        return v;
    }

    static bool TryAxisRect(Vector3[] world, out int axis, out float plane, out int a, out int b, out float a0, out float a1, out float b0, out float b1)
    {
        axis = -1;
        plane = a0 = a1 = b0 = b1 = 0f;
        a = b = 0;
        if (world == null || world.Length < 3)
            return false;
        float[] min = { float.MaxValue, float.MaxValue, float.MaxValue };
        float[] max = { float.MinValue, float.MinValue, float.MinValue };
        for (int i = 0; i < world.Length; i++)
        {
            for (int ax = 0; ax < 3; ax++)
            {
                float c = AxisOf(world[i], ax);
                if (c < min[ax]) min[ax] = c;
                if (c > max[ax]) max[ax] = c;
            }
        }
        int thin = 0;
        axis = -1;
        for (int ax = 0; ax < 3; ax++)
        {
            if (max[ax] - min[ax] <= 2f)
            {
                thin++;
                axis = ax;
                plane = (min[ax] + max[ax]) * 0.5f;
            }
        }
        if (thin != 1)
            return false;
        a = axis == 0 ? 1 : 0;
        b = axis == 2 ? 1 : 2;
        a0 = min[a]; a1 = max[a];
        b0 = min[b]; b1 = max[b];
        return a1 - a0 > 2f && b1 - b0 > 2f;
    }

    static bool BrushUsable(Brush b)
    {
        if (b == null || b.position == Vector3.zero || b.model?.poly?.polyData == null)
            return false;
        if (b.polyFlags != null && (b.polyFlags.Contains("PF_Invisible") || b.polyFlags.Contains("PF_NotSolid")))
            return false;
        foreach (var pd in b.model.poly.polyData)
        {
            if (pd?.vertices == null) continue;
            foreach (var v in pd.vertices)
                if (Mathf.Abs(v.x) > 60000 || Mathf.Abs(v.y) > 60000 || Mathf.Abs(v.z) > 60000)
                    return false;
        }
        return true;
    }

    static List<BspPortal> CollectPortalCaps(Brush[] brushes)
    {
        var list = new List<BspPortal>();
        if (brushes == null)
            return list;
        foreach (Brush b in brushes)
        {
            if (!BrushUsable(b) || b.csgOper != "CSG_Subtract")
                continue;
            Vector3 basePos = b.position - b.prePivot;
            float[] bmin = { float.MaxValue, float.MaxValue, float.MaxValue };
            float[] bmax = { float.MinValue, float.MinValue, float.MinValue };
            var worlds = new List<Vector3[]>();
            foreach (PolyData pd in b.model.poly.polyData)
            {
                if (pd?.vertices == null || pd.vertices.Length < 3)
                {
                    worlds.Add(null);
                    continue;
                }
                var w = new Vector3[pd.vertices.Length];
                for (int i = 0; i < pd.vertices.Length; i++)
                {
                    w[i] = basePos + pd.vertices[i];
                    for (int ax = 0; ax < 3; ax++)
                    {
                        float c = AxisOf(w[i], ax);
                        if (c < bmin[ax]) bmin[ax] = c;
                        if (c > bmax[ax]) bmax[ax] = c;
                    }
                }
                worlds.Add(w);
            }
            for (int p = 0; p < b.model.poly.polyData.Length; p++)
            {
                PolyData pd = b.model.poly.polyData[p];
                if (pd == null || worlds[p] == null)
                    continue;
                if (string.IsNullOrEmpty(pd.texture) || pd.texture.IndexOf("alpha", System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                if (!TryAxisRect(worlds[p], out int axis, out float plane, out int a, out int bv, out float a0, out float a1, out float b0, out float b1))
                    continue;
                if (Mathf.Abs(a0 - bmin[a]) > 8f || Mathf.Abs(a1 - bmax[a]) > 8f
                    || Mathf.Abs(b0 - bmin[bv]) > 8f || Mathf.Abs(b1 - bmax[bv]) > 8f)
                    continue;
                list.Add(new BspPortal
                {
                    Brush = b.name,
                    Axis = axis,
                    Plane = plane,
                    A = a,
                    B = bv,
                    A0 = a0,
                    A1 = a1,
                    B0 = b0,
                    B1 = b1
                });
            }
        }
        return list;
    }

    /// <summary>
    /// Subtract brushes are emitted as solid sheets, so an alpha cap that is the
    /// doorway opening stays as a wall. Drop that cap and cut it out of the coplanar face behind it.
    /// Null means "this polygon is the cap". An empty list means the face was the hole itself.
    /// </summary>
    static List<Vector3[]> PortalPieces(Brush b, PolyData pd, Vector3 basePos, string tex, List<BspPortal> portals)
    {
        var single = new List<Vector3[]> { pd.vertices };
        if (portals == null || portals.Count == 0 || pd.vertices == null)
            return single;
        if (tex != null && tex.IndexOf("ZonePortal", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return single;
        var world = new Vector3[pd.vertices.Length];
        for (int i = 0; i < pd.vertices.Length; i++)
            world[i] = basePos + pd.vertices[i];
        if (!TryAxisRect(world, out int axis, out float plane, out int a, out int bv, out float a0, out float a1, out float b0, out float b1))
            return single;

        bool alphaCap = b.csgOper == "CSG_Subtract"
            && tex != null
            && tex.IndexOf("alpha", System.StringComparison.OrdinalIgnoreCase) >= 0;
        BspPortal hit = default;
        bool found = false;
        for (int i = 0; i < portals.Count; i++)
        {
            BspPortal portal = portals[i];
            if (portal.Axis != axis || Mathf.Abs(portal.Plane - plane) > 4f)
                continue;
            if (portal.A != a)
                continue;
            bool inside = portal.A0 >= a0 - 4f && portal.A1 <= a1 + 4f
                && portal.B0 >= b0 - 4f && portal.B1 <= b1 + 4f;
            if (!inside)
                continue;
            if (alphaCap && portal.Brush == b.name
                && Mathf.Abs(portal.A0 - a0) < 8f && Mathf.Abs(portal.A1 - a1) < 8f
                && Mathf.Abs(portal.B0 - b0) < 8f && Mathf.Abs(portal.B1 - b1) < 8f)
                return null;
            hit = portal;
            found = true;
            break;
        }
        if (!found)
            return single;

        bool same = Mathf.Abs(hit.A0 - a0) < 8f && Mathf.Abs(hit.A1 - a1) < 8f
            && Mathf.Abs(hit.B0 - b0) < 8f && Mathf.Abs(hit.B1 - b1) < 8f;
        if (same)
            return new List<Vector3[]>();

        float pa0 = Mathf.Max(hit.A0, a0);
        float pa1 = Mathf.Min(hit.A1, a1);
        float pb0 = Mathf.Max(hit.B0, b0);
        float pb1 = Mathf.Min(hit.B1, b1);
        var frame = new List<Vector3[]>();
        void Add(float ua0, float ua1, float ub0, float ub1)
        {
            if (ua1 - ua0 < 2f || ub1 - ub0 < 2f)
                return;
            Vector3 c0 = WithAxis(axis, plane, a, ua0, bv, ub0) - basePos;
            Vector3 c1 = WithAxis(axis, plane, a, ua1, bv, ub0) - basePos;
            Vector3 c2 = WithAxis(axis, plane, a, ua1, bv, ub1) - basePos;
            Vector3 c3 = WithAxis(axis, plane, a, ua0, bv, ub1) - basePos;
            frame.Add(new[] { c0, c1, c2, c3 });
        }
        Add(a0, a1, b0, pb0);
        Add(a0, a1, pb1, b1);
        Add(a0, pa0, pb0, pb1);
        Add(pa1, a1, pb0, pb1);
        return frame;
    }
}
#endif