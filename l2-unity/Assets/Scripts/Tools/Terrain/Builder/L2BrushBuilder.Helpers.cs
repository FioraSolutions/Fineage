#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fineage.TerrainBsp;
using UnityEditor;
using UnityEngine;

public partial class L2BrushBuilder
{
    public static GameObject BuildBrushesFromT3d(string t3dAbsolutePath) {
        var bspBrushes = L2T3DBrushParser.ParseFile(t3dAbsolutePath);
        string tileAbs = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetFullPath(t3dAbsolutePath))).Replace('\\', '/');
        string dataAbs = Path.GetFullPath(Application.dataPath).Replace('\\', '/');
        if (tileAbs.StartsWith(dataAbs))
            return BuildMerged(bspBrushes, "Assets" + tileAbs.Substring(dataAbs.Length));
        return BuildMerged(bspBrushes, null);
    }

    public static GameObject BuildMerged(IReadOnlyList<L2BspBrush> brushes, string tileAssetFolder) {
        var matCache = new Dictionary<string, Material>();
        var subs = new Dictionary<Material, List<int>>();
        var colliderSubs = new List<int>();
        var verts = new List<Vector3>();
        var norms = new List<Vector3>();
        var uvs = new List<Vector2>();
        int polyN = 0, skippedBrush = 0, carved = 0, subtractSkipped = 0;
        var portals = L2BspCsgCarver.CollectPortalCaps(brushes);
        // UE2 CSG order: a subtract cuts additive brushes that precede it, not ones added later.
        int brushCount = brushes != null ? brushes.Count : 0;
        for (int bi = 0; bi < brushCount; bi++) {
            L2BspBrush b = brushes[bi];
            string skip = L2BspCsgCarver.UnusableReason(b);
            if (skip != null) {
                skippedBrush++;
                if (skip != "brush flags not rendered")
                    Debug.LogError("[Brushes] skipped brush " + (b != null ? b.name : "null") + ": " + skip
                        + " (tile " + tileAssetFolder + ")");
                continue;
            }
            if (string.Equals(b.csgOper, L2BspCsgCarver.CsgSubtract, StringComparison.Ordinal)) {
                subtractSkipped++;
                continue;
            }
            if (!string.Equals(b.csgOper, L2BspCsgCarver.CsgAdd, StringComparison.Ordinal)) {
                Debug.LogError("[Brushes] Unknown CsgOper on brush " + b.name + ": " + b.csgOper + " (tile " + tileAssetFolder + ")");
                skippedBrush++;
                continue;
            }
            Vector3 basePos = b.position - b.prePivot;
            var subtracts = new List<L2BspAabb>();
            for (int later = bi + 1; later < brushCount; later++)
                L2BspCsgCarver.AccumulateSubtract(brushes[later], subtracts);
            foreach (L2BspPoly pd in b.polygons) {
                if (pd.vertices == null || pd.vertices.Length < 3) continue;
                if (!L2BspPolyFlags.ShouldRenderPolygon(pd.polyFlags, pd.texture)) continue;
                string tex = pd.texture;
                if (!L2BspPolyFlags.IsBrushTextureRenderable(tex)) continue;
                if (!matCache.TryGetValue(tex, out var mat)) {
                    mat = FindMaterial(tex);
                    if (mat == null || mat.name == "BrushFallbackGray")
                        Debug.LogError("[Brushes] No material for BSP texture " + tex + " on brush " + b.name
                                       + " (tile " + tileAssetFolder + ")");
                    matCache[tex] = mat;
                }
                float tw = 256f, th = 256f;
                var mt = mat != null && mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : mat != null ? mat.mainTexture : null;
                if (mt != null) { tw = mt.width; th = mt.height; }
                List<Vector3[]> pieces = L2BspCsgCarver.CarveFacePieces(b, pd, basePos, portals, subtracts);
                if (pieces == null)
                    continue;
                if (pieces.Count == 0)
                    continue;
                if (pieces.Count != 1)
                    carved++;
                bool collide = L2BspPolyFlags.ShouldCollide(pd.polyFlags);
                for (int piece = 0; piece < pieces.Count; piece++) {
                    Vector3[] local = pieces[piece];
                    if (local == null || local.Length < 3) continue;
                    int n = local.Length;
                    var w = new Vector3[n];
                    for (int i = 0; i < n; i++)
                        w[i] = VectorUtils.ConvertPosToUnity(basePos + local[i]) * L2TerrainGeneratorTool.MAP_SCALE;
                    Vector3 nrm = Vector3.Cross(w[1] - w[0], w[2] - w[0]);
                    for (int i = 3; nrm.sqrMagnitude < 1e-10f && i < n; i++)
                        nrm = Vector3.Cross(w[1] - w[0], w[i] - w[0]);
                    if (nrm.sqrMagnitude < 1e-10f) continue;
                    nrm.Normalize();
                    if (!subs.TryGetValue(mat, out var tris)) subs[mat] = tris = new List<int>();
                    int colliderStart = verts.Count;
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
                    if (collide) {
                        for (int i = colliderStart; i < verts.Count; i++)
                            colliderSubs.Add(i);
                    }
                    polyN++;
                }
            }
        }
        GameObject root = new GameObject("Brushes");
        if (polyN == 0) {
            Debug.Log("[Brushes] no BSP polys (" + brushes.Count + " brushes, skipped " + skippedBrush
                      + ", subtract brushes kept for CSG only: " + subtractSkipped + ")");
            return root;
        }

        var mesh = new Mesh { name = (tileAssetFolder != null ? Path.GetFileName(tileAssetFolder) : "tile") + "_BSP", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.SetVertices(verts); mesh.SetNormals(norms); mesh.SetUVs(0, uvs);
        var mats = subs.Keys.ToList();
        mesh.subMeshCount = mats.Count;
        for (int i = 0; i < mats.Count; i++) mesh.SetTriangles(subs[mats[i]], i);
        mesh.RecalculateBounds(); mesh.RecalculateTangents();

        Mesh colliderMesh = mesh;
        if (colliderSubs.Count == 0) {
            Debug.LogWarning("[Brushes] no solid BSP collision polygons; MeshCollider uses full render mesh");
        } else if (colliderSubs.Count > 0 && colliderSubs.Count < verts.Count) {
            colliderMesh = new Mesh { name = mesh.name + "_Collider", indexFormat = mesh.indexFormat };
            colliderMesh.SetVertices(verts);
            colliderMesh.SetNormals(norms);
            colliderMesh.SetTriangles(colliderSubs, 0);
            colliderMesh.RecalculateBounds();
        }

        if (!string.IsNullOrEmpty(tileAssetFolder)) {
            string meshPath = tileAssetFolder + "/Brushes_Mesh.asset";
            if (AssetDatabase.LoadAssetAtPath<Mesh>(meshPath) != null) AssetDatabase.DeleteAsset(meshPath);
            AssetDatabase.CreateAsset(mesh, meshPath);
            AssetDatabase.SaveAssets();
        }

        var go = new GameObject("BSP");
        go.transform.SetParent(root.transform, false);
        int layer = LayerMask.NameToLayer("Brush"); if (layer < 0) layer = LayerMask.NameToLayer("StaticMesh");
        if (layer >= 0) { go.layer = layer; root.layer = layer; }
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
        go.AddComponent<MeshCollider>().sharedMesh = colliderMesh;
        go.isStatic = true;
        Debug.Log("[Brushes] BSP polys " + polyN + ", carve cuts " + carved + ", portal caps " + portals.Count
                  + ", subtract volumes " + subtracts.Count + ", materials " + mats.Count + ", brushes " + brushes.Count
                  + " (skipped " + skippedBrush + ", subtract-not-rendered " + subtractSkipped + ")");
        return root;
    }
}
#endif