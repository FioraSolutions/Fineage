#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class L2BrushBuilder {
    [MenuItem("Shnok/[Brush] (JSON) Build brushes")]
    static void ImportBrushTextures() {
        string title = "Select Brush list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            // Debug.Log("Selected file: " + fileToProcess);

            Brush[] brushes = L2JSONBrushImporter.ParseBrushFile(fileToProcess);

            Build(brushes);
        }
    }

    [MenuItem("Shnok/5. [Brush] (T3D) Build brushes")]
    static void ImportBrushTexturesT3D() {
        string title = "Select T3D file";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            BuildBrushesFromT3d(fileToProcess);
        }
    }

    public static GameObject BuildBrushesFromT3d(string t3dAbsolutePath) {
        Brush[] brushes = L2T3DInfoParser.ParseBrushInfo(t3dAbsolutePath).ToArray();
        // <tile>/Meta/<tile>.t3d -> Assets/.../Maps/<tile>
        string tileAbs = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetFullPath(t3dAbsolutePath))).Replace('\\', '/');
        string dataAbs = Path.GetFullPath(Application.dataPath).Replace('\\', '/');
        if (tileAbs.StartsWith(dataAbs))
            return BuildMerged(brushes, "Assets" + tileAbs.Substring(dataAbs.Length));
        return Build(brushes);
    }

    /// <summary>
    /// Interlude BSP (tools/interlude/BrushDump.java -> Brush actors in the tile t3d): one merged Mesh asset
    /// (submesh per material, UVs from UE TextureU/V, both windings so decks render + collide from either side)
    /// with a MeshCollider on layer Brush. The old ProBuilder path left meshes unsaved in the prefab and no colliders.
    /// Full CSG is not evaluated. <c>CSG_Subtract</c> brushes are not emitted as solid.
    /// Thin ZonePortal doorway subtracts cull only near-vertical ADD wall tris (opens guild/shop arches
    /// without eating terrace floors / railings — large room subtracts are excluded).
    /// </summary>
    public static GameObject BuildMerged(Brush[] brushes, string tileAssetFolder) {
        // Narrow doorway-only cull (EV baseline that opened guild/armory without eating railings).
        const bool cullDoorwayWalls = true;

        var matCache = new Dictionary<string, Material>();
        var subs = new Dictionary<Material, List<int>>();
        var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>();
        int polyN = 0, skippedBrush = 0, skippedSubtract = 0, culledBySubtract = 0, doorwaySubtracts = 0;

        var subtractBounds = new List<Bounds>();
        foreach (Brush b in brushes) {
            if (!string.Equals(b.csgOper, "CSG_Subtract", System.StringComparison.OrdinalIgnoreCase)) continue;
            if (b.position == Vector3.zero || b.model?.poly?.polyData == null) continue;
            skippedSubtract++;
            if (cullDoorwayWalls && TryDoorwaySubtractBounds(b, out Bounds bb))
            {
                subtractBounds.Add(bb);
                doorwaySubtracts++;
            }
        }

        foreach (Brush b in brushes) {
            if (b.position == Vector3.zero || b.model?.poly?.polyData == null) { skippedBrush++; continue; }
            if (b.polyFlags != null && (b.polyFlags.Contains("PF_Invisible") || b.polyFlags.Contains("PF_NotSolid"))) { skippedBrush++; continue; }
            if (string.Equals(b.csgOper, "CSG_Subtract", System.StringComparison.OrdinalIgnoreCase))
            {
                skippedBrush++;
                continue;
            }
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
                int n = pd.vertices.Length;
                var w = new Vector3[n];
                Vector3 center = Vector3.zero;
                for (int i = 0; i < n; i++) {
                    w[i] = VectorUtils.ConvertPosToUnity(basePos + pd.vertices[i]) * L2TerrainGeneratorTool.MAP_SCALE;
                    center += w[i];
                }
                center /= n;
                Vector3 nrm = Vector3.Cross(w[1] - w[0], w[2] - w[0]);
                for (int i = 3; nrm.sqrMagnitude < 1e-10f && i < n; i++) nrm = Vector3.Cross(w[1] - w[0], w[i] - w[0]);
                if (nrm.sqrMagnitude < 1e-10f) continue;
                nrm.Normalize();
                // Optional narrow doorway wall cull (off by default — see cullDoorwayWalls).
                if (cullDoorwayWalls && Mathf.Abs(nrm.y) < 0.2f && IsInsideAny(subtractBounds, center))
                { culledBySubtract++; continue; }
                if (!matCache.TryGetValue(tex, out var mat)) { mat = FindMaterial(tex); matCache[tex] = mat; }
                float tw = 256f, th = 256f;
                var mt = mat != null && mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : mat != null ? mat.mainTexture : null;
                if (mt != null) { tw = mt.width; th = mt.height; }
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
        Debug.Log("[Brushes] BSP polys " + polyN + ", materials " + mats.Count
            + ", brushes " + brushes.Length
            + " (skipped " + skippedBrush + ", subtract " + skippedSubtract
            + ", doorwaySubtracts " + doorwaySubtracts
            + ", culledBySubtract " + culledBySubtract + ", subtractBounds " + subtractBounds.Count
            + ") -> " + meshPath);
        return root;
    }

    /// <summary>
    /// Doorway CSG_Subtract brushes are thin cutting slabs (~2 UU deep, ~door width/height). Large room
    /// subtracts must not drive AABB cull — they punched holes in EV railings/walls.
    /// </summary>
    static bool TryDoorwaySubtractBounds(Brush b, out Bounds bounds) {
        bounds = new Bounds();
        bool any = false;
        Vector3 basePos = b.position - b.prePivot;
        float scale = L2TerrainGeneratorTool.MAP_SCALE;
        foreach (PolyData pd in b.model.poly.polyData) {
            if (pd.vertices == null) continue;
            foreach (var v in pd.vertices) {
                if (Mathf.Abs(v.x) > 60000 || Mathf.Abs(v.y) > 60000 || Mathf.Abs(v.z) > 60000) return false;
                Vector3 u = VectorUtils.ConvertPosToUnity(basePos + v) * scale;
                if (!any) { bounds = new Bounds(u, Vector3.zero); any = true; }
                else bounds.Encapsulate(u);
            }
        }
        if (!any) return false;
        Vector3 size = bounds.size;
        float minAx = Mathf.Min(size.x, Mathf.Min(size.y, size.z));
        float maxAx = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
        // Thin slab + door-sized: ~2 UU depth before scale ≈ 0.04u; after convert door ~1.5–3u wide/tall.
        if (minAx > 0.55f || maxAx > 5.5f) return false;
        const float minAxis = 1.0f;
        Vector3 expand = new Vector3(
            Mathf.Max(0f, (minAxis - size.x) * 0.5f) + 0.08f,
            Mathf.Max(0f, (minAxis - size.y) * 0.5f) + 0.08f,
            Mathf.Max(0f, (minAxis - size.z) * 0.5f) + 0.08f);
        bounds.Expand(expand * 2f);
        return true;
    }

    static bool IsInsideAny(List<Bounds> list, Vector3 p) {
        for (int i = 0; i < list.Count; i++)
            if (list[i].Contains(p)) return true;
        return false;
    }

    static Material _fallback;
    static Material FindMaterial(string texture) {
        string path = TextureUtils.GetMaterialPath(texture);
        Material m = path != null ? AssetDatabase.LoadAssetAtPath<Material>(path) : null;
        if (m != null) return m;
        string[] parts = texture.Split('.');
        string pkg = parts[0], file = parts[parts.Length - 1];
        string best = null;
        foreach (var g in AssetDatabase.FindAssets(file + " t:Material", new[] { "Assets/Resources/Data/Textures" })) {
            string p = AssetDatabase.GUIDToAssetPath(g);
            if (!string.Equals(Path.GetFileNameWithoutExtension(p), file, System.StringComparison.OrdinalIgnoreCase)) continue;
            if (p.IndexOf("/" + pkg + "/", System.StringComparison.OrdinalIgnoreCase) >= 0) { best = p; break; }
            if (best == null) best = p;
        }
        if (best != null) return AssetDatabase.LoadAssetAtPath<Material>(best);
        // texture exported but no material yet -> make a URP Lit one next to it
        foreach (var g in AssetDatabase.FindAssets(file + " t:Texture2D", new[] { "Assets/Resources/Data/Textures" })) {
            string tp = AssetDatabase.GUIDToAssetPath(g);
            if (!string.Equals(Path.GetFileNameWithoutExtension(tp), file, System.StringComparison.OrdinalIgnoreCase)) continue;
            string dir = Path.GetDirectoryName(tp).Replace('\\', '/') + "/Materials";
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder(Path.GetDirectoryName(tp).Replace('\\', '/'), "Materials");
            var nm = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var t = AssetDatabase.LoadAssetAtPath<Texture2D>(tp);
            nm.SetTexture("_BaseMap", t); nm.SetTexture("_MainTex", t); nm.SetFloat("_Smoothness", 0f);
            AssetDatabase.CreateAsset(nm, dir + "/" + file + ".mat");
            return nm;
        }
        Debug.LogWarning("[Brushes] no texture/material for " + texture + " (gray)");
        if (_fallback == null) {
            _fallback = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Data/Textures/BrushFallbackGray.mat");
            if (_fallback == null) {
                _fallback = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                _fallback.SetColor("_BaseColor", new Color(0.62f, 0.58f, 0.5f)); _fallback.SetFloat("_Smoothness", 0f);
                AssetDatabase.CreateAsset(_fallback, "Assets/Resources/Data/Textures/BrushFallbackGray.mat");
            }
        }
        return _fallback;
    }

    static GameObject Build(Brush[] brushes) {
        GameObject brushContainer = new GameObject("Brushes");

        foreach(Brush b in brushes) {
            if(b.position == Vector3.zero) {
                // Debug.LogWarning(b.name + " position is null");
                continue;
            }
            if(b.polyFlags != null) {
                List<string> polyFlags = new List<string>(b.polyFlags);
                if (polyFlags.Contains("PF_Invisible")) {
                    continue;
                }
                if (polyFlags.Contains("PF_NotSolid")) {
                    continue;
                }
            }

            GameObject brush = new GameObject(b.name);
            brush.transform.parent = brushContainer.transform;
            brush.transform.position = VectorUtils.ConvertPosToUnity(b.position) - VectorUtils.ConvertPosToUnity(b.prePivot);

            // Debug.Log(b.name);
            Model model = b.model;
            Poly poly = model.poly;

            for(int i = 0; i < poly.polyData.Length; i++) {
                PolyData polyData = poly.polyData[i];

                // Adjust verticles
                for(int p = 0; p < polyData.vertices.Length; p++) {
                    polyData.vertices[p] = VectorUtils.ConvertPosToUnity(polyData.vertices[p]);
                }

                List<string> pPolyFlags = new List<string>(polyData.polyFlags);

                // Skip invisible faces
                if(pPolyFlags.Contains("PF_Unlit") || pPolyFlags.Contains("PF_Invisible") || pPolyFlags.Contains("PF_NotSolid")) {
                    continue;
                }
                
                if(b.csgOper == "CSG_Subtract") {
                    // Only draw bottom face
                    // if (i != poly.polyData.Length - 1) {
                    //continue;
                    //}
                }

                //GameObject mesh = createMesh(b.csgOper, polyData);

                GameObject mesh = createProbuilderMesh(b.csgOper, polyData, i);
                mesh.transform.parent = brush.transform;
                mesh.transform.localPosition = Vector3.zero;
            }
        }

        return brushContainer;
    }

    static GameObject createProbuilderMesh(string csgOper, PolyData polyData, int index) {

        Material material = GetMaterialForTexture(polyData.texture);

        Vector3 adjustedNormal = VectorUtils.ConvertToUnityUnscaled(polyData.normal);
        Vector3 adjustedU = VectorUtils.ConvertToUnityUnscaled(polyData.textureU);
        Vector3 adjustedV = VectorUtils.ConvertToUnityUnscaled(polyData.textureV);

        Quaternion rt = Quaternion.FromToRotation(Vector3.forward, adjustedNormal);
        Vector3 rotatedU = rt * adjustedU;
        Vector3 rotatedV = rt * adjustedV;
        if(adjustedNormal.y < 0) {
            rotatedU.x = -rotatedU.x;
        } else {
            rotatedU.y = -rotatedU.y;
        }

        // Create Vertices
        List<Vertex> vertexList = new List<Vertex>();
        foreach(var v in polyData.vertices) {
            Vertex vertex = new Vertex();
            vertex.position = v;
            vertexList.Add(vertex);
        }

        // Create faces
        Face[] faces = new Face[1];
        Face face = new Face(GenerateTris(csgOper, polyData));

        AutoUnwrapSettings aus = new AutoUnwrapSettings {
            anchor = AutoUnwrapSettings.Anchor.UpperLeft,
            scale = Vector2.one,
            offset = Vector2.zero,
            rotation = 0,
            fill = AutoUnwrapSettings.Fill.Tile,
            useWorldSpace = false
        };
        face.uv = aus;
        face.manualUV = true;
        faces[0] = face;

        // Create empty sharedVertices and sharedTextures lists
        List<SharedVertex> sharedVertices = new List<SharedVertex>();
        List<SharedVertex> sharedTextures = new List<SharedVertex>();

        // Create Mesh
        ProBuilderMesh pbm = ProBuilderMesh.Create(
            vertexList,
            faces,
            sharedVertices,
            sharedTextures
        );

        List<Vector4> uvs = new List<Vector4>();
        for(int i = 0; i < vertexList.Count; i++) {
            uvs.Add(new Vector4(rotatedU.x, rotatedU.y, rotatedV.x, rotatedV.y));
        }
        pbm.SetUVs(0, uvs);
        pbm.Refresh();

        Material[] sharedMaterials = pbm.GetComponent<Renderer>().sharedMaterials;
        sharedMaterials[0] = material;
        pbm.GetComponent<Renderer>().sharedMaterials = sharedMaterials;

        pbm.ToMesh();
        pbm.Refresh();
        pbm.RebuildWithPositionsAndFaces(polyData.vertices, faces);

        pbm.gameObject.transform.name = index.ToString();
        return pbm.gameObject;
    }

    private static int[] GenerateTris(string csgOper, PolyData polyData) {
        int[] triangles;

        if(csgOper == "CSG_Subtract") {
            if(polyData.vertices.Length == 4) {
                triangles = new int[6]
                {
                    2, 1, 0, // First triangle (top-left, top-right, bottom-left)
                    0, 3, 2  // Second triangle (bottom-left, top-right, bottom-right)
                };
            } else {
                triangles = new int[3]
                {
                    2, 1, 0, // First triangle (top-left, top-right, bottom-left)
                };
            }
        } else {
            if(polyData.vertices.Length == 4) {
                triangles = new int[6]
                {
                    0, 1, 2, // First triangle (top-left, top-right, bottom-left)
                    2, 3, 0  // Second triangle (bottom-left, top-right, bottom-right)
                };
            } else {
                triangles = new int[3]
                {
                    0, 1, 2, // First triangle (top-left, top-right, bottom-left)
                };
            }
        }

        return triangles;
    }

    private static Material GetMaterialForTexture(string texture) {
        string materialPath = TextureUtils.GetMaterialPath(texture);

        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if(material == null) {
            material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefab/Red.mat");
            // Debug.LogError("Missing material for " + texture);
        }

        return material;
    }
}
#endif