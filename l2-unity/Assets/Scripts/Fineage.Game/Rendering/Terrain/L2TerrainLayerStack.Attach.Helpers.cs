using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public sealed partial class L2TerrainLayerStack
{
    static void AttachAll()
    {
        MeshRenderer[] renderers = UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        int attached = 0;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (TryAttach(renderers[i]))
                attached++;
        }

        if (!_loggedAttach)
        {
            _loggedAttach = true;
            // Debug.Log("[L2TerrainLayerStack] attach scan renderers=" + renderers.Length + " terrain=" + attached);
        }
    }

    public static bool TryAttach(Renderer renderer)
    {
        if (renderer == null)
            return false;
        Material src = renderer.sharedMaterial;
        if (!IsTerrainSource(src))
            return false;
        if (renderer.GetComponent<L2TerrainLayerStack>() != null)
            return true;
        renderer.gameObject.AddComponent<L2TerrainLayerStack>();
        return true;
    }

    public static bool IsTerrainSource(Material mat)
    {
        if (mat == null || mat.shader == null)
            return false;
        string n = mat.shader.name;
        return n == BaseShaderName
            || n == "L2/World/TerrainModulate2X"
            || n == "L2/World/TerrainSplatModulate2X";
    }

    static bool IsSkippedOverlay(Texture tex)
    {
        if (tex == null)
            return true;
        string n = tex.name;
        if (string.IsNullOrEmpty(n))
            return false;
        if (n.IndexOf("Water", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
        if (n.IndexOf("SL_WR", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
        if (n.EndsWith("_WR", System.StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    static Texture ReadTex(Material src, string name)
    {
        if (src == null || string.IsNullOrEmpty(name))
            return null;
        if (!src.HasProperty(name))
            return null;
        return src.GetTexture(name);
    }
}
