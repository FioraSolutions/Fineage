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
        for (int layer = FirstOverlay; layer <= LastOverlay; layer++)
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

        for (int layer = FirstOverlay; layer <= LastOverlay; layer++)
        {
            bool on = !src.HasProperty("_Layer_" + layer + "_Enabled")
                || src.GetFloat("_Layer_" + layer + "_Enabled") > 0.5f;
            Texture tex = ReadTex(src, "_Layer_" + layer);
            Texture mask = ReadTex(src, "_LayerMask_" + layer);
            if (!on || tex == null || mask == null || IsSkippedOverlay(tex))
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

            mat.name = src.name + "_Overlay" + layer + "_" + tex.name;
            mat.SetTexture(LayerTexId, tex);
            mat.SetTexture(LayerMaskId, mask);
            mat.SetFloat(LayerIndexId, layer);
            mat.SetFloat(SplatSliceId, layer - 1);
            if (src.HasProperty("_Layer_" + layer + "_UV"))
                mat.SetVector(LayerUvId, src.GetVector("_Layer_" + layer + "_UV"));
            else
                mat.SetVector(LayerUvId, new Vector4(1f, 1f, 0f, 0f));
            mat.SetFloat(WorldUvId, worldUv);
            mat.SetFloat(CullId, cull);
            mat.enableInstancing = false;
            _active[layer] = true;
        }
    }
}
