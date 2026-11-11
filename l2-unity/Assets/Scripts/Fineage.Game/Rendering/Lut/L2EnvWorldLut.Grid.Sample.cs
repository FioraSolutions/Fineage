using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    static void PushIntensityGrid(float hours, float fallback01)
    {
        if (LoadBaked17_25Intensity)
        {
            PushBaked17_25(hours, fallback01);
            return;
        }

        if (!LoadStoredIntensityMasks)
        {
            EnsureUnityTreePreview();
            if (_unityTreePreview != null)
            {
                if (!_unityTreePreviewUploaded)
                    UploadUnityTreePreview();
                return;
            }

            PushOpenIntensity();
            return;
        }

        EnsureIntensityGrid();
        EnsureNeighborGrids();
        byte fill = (byte)Mathf.Clamp(Mathf.RoundToInt(fallback01 * 255f), 0, 255);
        int a;
        int b;
        float u;
        PickIntensitySlices(IntensitySlices, hours, out a, out b, out u);
        Shader.SetGlobalFloat(TerrainLightDebugId, ShowTerrainLightDebug ? 1f : 0f);
        bool primaryClean = _intensityTex != null && a == _gridSliceA && b == _gridSliceB && fill == _gridFill;
        bool neighborsClean = NeighborsClean(hours, fill);
        if (primaryClean && neighborsClean)
            return;

        if (!primaryClean)
            UploadPrimaryGrid(a, b, u, fill);
        if (!neighborsClean)
            UploadNeighborGrids(hours, fill);
    }

    static void UploadPrimaryGrid(int a, int b, float u, byte fill)
    {

        int n = IntensityGridSize;
        var pixels = new byte[n * n * 4];
        if (a < 0 || IntensitySlices.Count == 0)
        {
            for (int i = 0; i < n * n; i++)
            {
                pixels[i * 4] = fill;
                pixels[i * 4 + 3] = 255;
            }
        }
        else
        {
            IntensitySlice sa = IntensitySlices[a];
            IntensitySlice sb = IntensitySlices[b];
            for (int i = 0; i < n * n; i++)
            {
                bool va = sa.valid[i] != 0;
                bool vb = sb.valid[i] != 0;
                byte value = fill;
                byte valid = 0;
                if (va && vb)
                {
                    float mixed = sa.intensity[i] + (sb.intensity[i] - sa.intensity[i]) * u;
                    int truncated = (int)mixed;
                    if (truncated > 255)
                        truncated = 255;
                    if (truncated < 0)
                        truncated = 0;
                    value = (byte)truncated;
                    valid = 255;
                }
                else if (va)
                {
                    value = sa.intensity[i];
                    valid = 255;
                }
                else if (vb)
                {
                    value = sb.intensity[i];
                    valid = 255;
                }

                pixels[i * 4] = value;
                pixels[i * 4 + 1] = valid;
                pixels[i * 4 + 3] = 255;
            }
        }

        if (_intensityTex == null)
        {
            _intensityTex = new Texture2D(n, n, TextureFormat.RGBA32, false, true)
            {
                name = "L2TerrainIntensityMap",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
            _intensityTex.hideFlags = HideFlags.HideAndDontSave;
        }

        _intensityTex.SetPixelData(pixels, 0);
        _intensityTex.Apply(false, false);
        Shader.SetGlobalTexture(TerrainIntensityMapId, _intensityTex);
        Shader.SetGlobalVector(TerrainIntensityOriginId, IntensityOrigin);
        _gridSliceA = a;
        _gridSliceB = b;
        _gridFill = fill;
    }

    static void PushBaked17_25(float hours, float fallback01)
    {
        Shader.SetGlobalFloat(TerrainLightDebugId, 0f);
        EnsureBaked17_25();
        if (!_bakedNeighborsCleared)
        {
            Shader.SetGlobalVector(TerrainIntensityOrigin1Id, Vector4.zero);
            Shader.SetGlobalVector(TerrainIntensityOrigin2Id, Vector4.zero);
            _bakedNeighborsCleared = true;
        }

        if (IntensitySlices.Count == 0)
        {
            PushOpenIntensity();
            return;
        }

        byte fill = (byte)Mathf.Clamp(Mathf.RoundToInt(fallback01 * 255f), 0, 255);
        int a;
        int b;
        float u;
        PickIntensitySlices(IntensitySlices, hours, out a, out b, out u);
        if (_intensityTex != null && a == _gridSliceA && b == _gridSliceB && fill == _gridFill)
            return;
        UploadPrimaryGrid(a, b, u, fill);
    }
}
