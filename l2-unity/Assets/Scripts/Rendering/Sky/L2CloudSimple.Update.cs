using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudSimple : MonoBehaviour
{
    static Mesh LoadPackageMesh(string resourcesPath)
    {
        Mesh direct = Resources.Load<Mesh>(resourcesPath);
        if (direct != null)
            return direct;

        Mesh[] all = Resources.LoadAll<Mesh>(resourcesPath);
        if (all == null || all.Length == 0)
            return null;

        Mesh best = null;
        int bestVerts = -1;
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] == null || all[i].vertexCount <= bestVerts)
                continue;
            best = all[i];
            bestVerts = all[i].vertexCount;
        }

        return best;
    }

    static void Wrap(Texture2D tex, TextureWrapMode u, TextureWrapMode v)
    {
        if (tex == null)
            return;
        tex.wrapModeU = u;
        tex.wrapModeV = v;
    }

    void EnsureChunk(ref CloudChunk chunk, string goName, Mesh source, int materialPasses)
    {
        if (source == null)
            return;

        bool needRebuild = chunk.mesh == null || chunk.mesh.name != source.name + "_cloud";
        if (needRebuild)
        {
            DestroyObj(chunk.mesh);
            chunk.mesh = BuildMultiPassMesh(source, materialPasses);
            if (chunk.mf != null)
                chunk.mf.sharedMesh = chunk.mesh;
        }

        if (chunk.tf != null)
            return;

        var go = new GameObject(goName);
        go.hideFlags = HideFlags.HideAndDontSave;
        ApplyCloudLayer(go);
        chunk.tf = go.transform;
        chunk.tf.SetParent(transform, false);

        chunk.mf = go.AddComponent<MeshFilter>();
        chunk.mf.sharedMesh = chunk.mesh;
        chunk.mr = go.AddComponent<MeshRenderer>();
        chunk.mr.shadowCastingMode = ShadowCastingMode.Off;
        chunk.mr.receiveShadows = false;
        chunk.mr.allowOcclusionWhenDynamic = false;
        chunk.mr.lightProbeUsage = LightProbeUsage.Off;
        chunk.mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
    }

    /// <summary>
    /// Package mesh has one active material; dual cloud sheets need N identical passes.
    /// </summary>
    static Mesh BuildMultiPassMesh(Mesh source, int passes)
    {
        var mesh = new Mesh { name = source.name + "_cloud" };
        mesh.hideFlags = HideFlags.DontSave;
        mesh.vertices = source.vertices;
        mesh.uv = source.uv;
        mesh.normals = source.normals;
        int n = source.vertexCount;
        var colors = new Color[n];
        // RenderDoc mesh_out_1.csv: FF in_Color0 is exactly white for every
        // cloud vertex. FBX may contain a stale/black color channel; ignore it.
        for (int i = 0; i < n; i++)
            colors[i] = Color.white;
        mesh.colors = colors;
        int[] tris = source.triangles;
        if (tris == null || tris.Length < 3)
        {
            // Prefer first non-empty submesh.
            for (int i = 0; i < source.subMeshCount; i++)
            {
                int[] sub = source.GetTriangles(i);
                if (sub != null && sub.Length >= 3)
                {
                    tris = sub;
                    break;
                }
            }
        }

        if (tris == null)
            tris = System.Array.Empty<int>();

        passes = Mathf.Max(1, passes);
        mesh.subMeshCount = passes;
        for (int i = 0; i < passes; i++)
            mesh.SetTriangles(tris, i, true);
        mesh.RecalculateBounds();
        return mesh;
    }

    static Material MakeMat(Shader shader, Texture2D tex, Color tint, int queue, bool alphaBlend, float meshRadius)
    {
        var mat = new Material(shader);
        mat.hideFlags = HideFlags.DontSave;
        mat.SetTexture("_MainTex", tex);
        mat.SetVector(TextureFactorId, new Vector4(tint.r, tint.g, tint.b, tint.a));
        mat.SetVector(SkyParamsId, new Vector4(88f, meshRadius, 0f, 0f));
        mat.SetFloat(SrcBlendId, alphaBlend ? (float)BlendMode.SrcAlpha : (float)BlendMode.One);
        mat.SetFloat(DstBlendId, alphaBlend ? (float)BlendMode.OneMinusSrcAlpha : (float)BlendMode.One);
        mat.renderQueue = queue;
        return mat;
    }

    static void ApplyCloudLayer(GameObject go)
    {
        if (go == null)
            return;
        if (L2FxCompositorRuntime.PreferGpuQueue)
            L2FxCompositorLayers.ApplyL2CloudsLayer(go);
        else
            go.layer = 0;
    }

    public void DrawBloomExclude(CommandBuffer cmd)
    {
        DrawChunkExclude(cmd, ref _domeA);
        DrawChunkExclude(cmd, ref _domeB);
    }

    static void DrawChunkExclude(CommandBuffer cmd, ref CloudChunk chunk)
    {
        if (cmd == null || chunk.mr == null || !chunk.mr.enabled || chunk.mats == null)
            return;

        for (int i = 0; i < chunk.mats.Length; i++)
        {
            Material mat = chunk.mats[i];
            if (mat == null)
                continue;
            int pass = mat.FindPass("BloomExclude");
            if (pass < 0)
                continue;
            cmd.DrawRenderer(chunk.mr, mat, i, pass);
        }
    }

    float ResolveSkyDistance(Camera cam)
    {
        float far = cam != null ? cam.farClipPlane : 500f;
        return Mathf.Clamp(far * 0.88f, 80f, 4000f);
    }

    Camera ResolveCamera()
    {
        if (CameraPacketServices.Controller != null)
        {
            Camera fromController = CameraPacketServices.Controller.GetComponent<Camera>();
            if (fromController == null)
                fromController = CameraPacketServices.Controller.GetComponentInChildren<Camera>();
            if (fromController != null && fromController.enabled && fromController.gameObject.activeInHierarchy)
            {
                _cam = fromController;
                return _cam;
            }
        }

        Camera main = Camera.main;
        if (main != null && main.enabled && main.gameObject.activeInHierarchy)
        {
            _cam = main;
            return _cam;
        }

        Camera[] cams = Camera.allCameras;
        for (int i = 0; i < cams.Length; i++)
        {
            Camera c = cams[i];
            if (c != null && c.enabled && c.gameObject.activeInHierarchy && !c.orthographic)
            {
                _cam = c;
                return _cam;
            }
        }

        return _cam;
    }

    void DestroyChunks()
    {
        DestroyChunk(ref _domeA);
        DestroyChunk(ref _domeB);
    }

    static void DestroyChunk(ref CloudChunk chunk)
    {
        if (chunk.mats != null)
        {
            for (int i = 0; i < chunk.mats.Length; i++)
                DestroyObj(chunk.mats[i]);
            chunk.mats = null;
        }

        DestroyObj(chunk.mesh);
        chunk.mesh = null;
        if (chunk.tf != null)
            DestroyObj(chunk.tf.gameObject);
        chunk.tf = null;
        chunk.mf = null;
        chunk.mr = null;
    }

    static void DestroyObj(UnityEngine.Object obj)
    {
        if (obj == null)
            return;
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
}
