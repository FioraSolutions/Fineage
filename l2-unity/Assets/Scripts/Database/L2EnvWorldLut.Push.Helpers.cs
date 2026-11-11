using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    static string UnityTreePreviewPath()
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "BakeOutput", "L2TerrainIntensityGrid_unitytree_preview.bin"));
    }

    static void EnsureUnityTreePreview()
    {
        if (_unityTreePreview != null)
            return;
        string path = UnityTreePreviewPath();
        if (!File.Exists(path))
            return;
        byte[] file = File.ReadAllBytes(path);
        if (file.Length != IntensityGridSize * IntensityGridSize)
            return;
        _unityTreePreview = file;
    }

    static void UploadUnityTreePreview()
    {
        EnsureUnityTreePreview();
        if (_unityTreePreview == null)
            return;

        int n = IntensityGridSize;
        var pixels = new byte[n * n * 4];
        for (int i = 0; i < n * n; i++)
        {
            pixels[i * 4] = _unityTreePreview[i];
            pixels[i * 4 + 1] = 255;
            pixels[i * 4 + 3] = 255;
        }

        if (_intensityTex == null || _intensityTex.width != n || _intensityTex.height != n)
        {
            if (_intensityTex != null)
                UnityEngine.Object.DestroyImmediate(_intensityTex);
            _intensityTex = new Texture2D(n, n, TextureFormat.RGBA32, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                name = "L2TerrainIntensityUnityTree",
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        _intensityTex.SetPixelData(pixels, 0);
        _intensityTex.Apply(false, false);
        Shader.SetGlobalTexture(TerrainIntensityMapId, _intensityTex);
        Shader.SetGlobalVector(TerrainIntensityOriginId, IntensityOrigin);
        Shader.SetGlobalVector(TerrainIntensityOrigin1Id, Vector4.zero);
        Shader.SetGlobalVector(TerrainIntensityOrigin2Id, Vector4.zero);
        Shader.SetGlobalFloat(TerrainLightDebugId, 0f);
        _unityTreePreviewUploaded = true;
        _openIntensityPushed = true;
    }

    static void PushOpenIntensity()
    {
        Shader.SetGlobalFloat(TerrainLightDebugId, 0f);
        if (_openIntensityPushed && _intensityTex != null)
            return;
        _openIntensityPushed = true;
        if (_intensityTex == null)
        {
            _intensityTex = new Texture2D(1, 1, TextureFormat.RGBA32, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                name = "L2TerrainIntensityOpen"
            };
            _intensityTex.SetPixel(0, 0, Color.white);
            _intensityTex.Apply(false, false);
        }

        Shader.SetGlobalTexture(TerrainIntensityMapId, _intensityTex);
        Shader.SetGlobalVector(TerrainIntensityOriginId, IntensityOrigin);
        Shader.SetGlobalVector(TerrainIntensityOrigin1Id, Vector4.zero);
        Shader.SetGlobalVector(TerrainIntensityOrigin2Id, Vector4.zero);
    }
}
