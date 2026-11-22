using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public sealed partial class L2TerrainLayerStack
{
    public static void DrawAll(CommandBuffer cmd)
    {
        if (cmd == null)
            return;
        for (int i = 0; i < Active.Count; i++)
        {
            L2TerrainLayerStack stack = Active[i];
            if (stack != null)
                stack.Draw(cmd);
        }
    }

    public void Draw(CommandBuffer cmd)
    {
        if (cmd == null || _renderer == null || _filter == null || !_renderer.enabled || !isActiveAndEnabled)
            return;
        Mesh mesh = _filter.sharedMesh;
        if (mesh == null)
            return;
        if (_layerShader == null)
            _layerShader = Shader.Find(LayerShaderName);
        Material src = _renderer.sharedMaterial;
        if (!IsTerrainSource(src) || _layerShader == null)
        {
            LogOnce("skip draw shaderFound=" + (_layerShader != null) + " mat=" + (src != null ? src.name : "null") +
                    " shader=" + (src != null && src.shader != null ? src.shader.name : "null"));
            return;
        }

        SyncOverlays(src);
        Matrix4x4 matrix = _renderer.localToWorldMatrix;
        int subCount = Mathf.Max(1, mesh.subMeshCount);
        int drawn = 0;
        for (int layer = FirstOverlay; layer <= _lastOverlay; layer++)
        {
            if (!_active[layer] || _overlayMats[layer] == null)
                continue;
            for (int sub = 0; sub < subCount; sub++)
                cmd.DrawMesh(mesh, matrix, _overlayMats[layer], sub, 0);
            drawn++;
        }

        LogOnce("overlays=" + drawn + " mesh=" + mesh.name + " mat=" + src.name +
                " shader=" + _layerShader.name);
    }

    void SyncOverlays(Material src)
    {
        float worldUv = src.HasProperty("_L2WorldUvPerTileUu") ? src.GetFloat("_L2WorldUvPerTileUu") : 128f;
        float cull = src.HasProperty("_Cull") ? src.GetFloat("_Cull") : 2f;

        Texture2DArray layerArray = src.HasProperty("_Layers") ? src.GetTexture("_Layers") as Texture2DArray : null;
        Texture2DArray splatArray = src.HasProperty("_Splatmaps") ? src.GetTexture("_Splatmaps") as Texture2DArray : null;
        int depth = 0;
        if (layerArray != null)
            depth = Mathf.Max(depth, layerArray.depth);
        if (splatArray != null)
            depth = Mathf.Max(depth, splatArray.depth);

        if (depth == 0)
        {
            int n = FirstOverlay;
            while (ReadTex(src, "_Layer_" + n) != null
                || ReadTex(src, "_LayerMask_" + n) != null
                || (src.HasProperty("_Layer_" + n + "_Enabled") && src.GetFloat("_Layer_" + n + "_Enabled") > 0.5f))
                n++;
            depth = n - FirstOverlay;
        }

        _lastOverlay = FirstOverlay + depth - 1;
        if (_lastOverlay < FirstOverlay - 1)
            _lastOverlay = FirstOverlay - 1;
        EnsureOverlayCapacity(Mathf.Max(_lastOverlay, FirstOverlay));

        for (int layer = FirstOverlay; layer <= _lastOverlay; layer++)
        {
            int slice = layer - 1;
            bool on = !src.HasProperty("_Layer_" + layer + "_Enabled")
                || src.GetFloat("_Layer_" + layer + "_Enabled") > 0.5f;
            Texture tex2d = ReadTex(src, "_Layer_" + layer);
            Texture mask2d = ReadTex(src, "_LayerMask_" + layer);
            bool useArrays = layerArray != null && splatArray != null
                && slice < layerArray.depth && slice < splatArray.depth;
            if (!on || (!useArrays && (tex2d == null || mask2d == null)))
            {
                _active[layer] = false;
                continue;
            }

            Material mat = _overlayMats[layer];
            if (mat == null || mat.shader != _layerShader)
            {
                if (mat != null)
                    DestroyOverlay(mat);
                mat = new Material(_layerShader);
                mat.hideFlags = HideFlags.HideAndDontSave;
                _overlayMats[layer] = mat;
            }

            string sliceName = useArrays
                ? (layerArray.name + "[" + slice + "]")
                : (tex2d != null ? tex2d.name : "overlay");
            mat.name = src.name + "_Overlay" + layer + "_" + sliceName;
            if (useArrays)
            {
                mat.SetTexture(LayersId, layerArray);
                mat.SetTexture(SplatmapsId, splatArray);
                mat.SetFloat(UseSplatArraysId, 1f);
            }
            else
            {
                mat.SetTexture(LayerTexId, tex2d);
                mat.SetTexture(LayerMaskId, mask2d);
                mat.SetFloat(UseSplatArraysId, 0f);
            }
            mat.SetFloat(LayerIndexId, layer);
            mat.SetFloat(SplatSliceId, slice);
            if (src.HasProperty("_Layer_" + layer + "_UV"))
                mat.SetVector(LayerUvId, src.GetVector("_Layer_" + layer + "_UV"));
            else
                mat.SetVector(LayerUvId, new Vector4(1f, 1f, 0f, 0f));
            mat.SetFloat(WorldUvId, worldUv);
            mat.SetFloat(CullId, cull);
            mat.enableInstancing = false;
            _active[layer] = true;
        }

        for (int layer = _lastOverlay + 1; layer < _active.Length; layer++)
            _active[layer] = false;
    }
}
