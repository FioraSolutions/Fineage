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

    static void EnsureBaked17_25()
    {
        if (_baked17Tried)
            return;
        _baked17Tried = true;
        _gridTried = true;
        if (!ReadIntensityGrid(Baked17_25IntensityFileName, IntensitySlices))
            Debug.LogWarning("[L2EnvWorldLut] 17_25 baked intensity grid was not loaded.");
    }

    static void EnsureIntensityGrid()
    {
        if (_gridTried)
            return;
        _gridTried = true;
        string fileName = UseStoredClientIntensityGrid ? StoredIntensityGridFileName : IntensityGridFileName;
        string path = Path.Combine(Application.streamingAssetsPath, "Data/Meta", fileName);
        if (!File.Exists(path))
        {
            // Debug.LogWarning("[L2EnvWorldLut] Intensity grid missing (" + fileName + ")");
            return;
        }

        byte[] file = File.ReadAllBytes(path);
        if (file.Length < 16 || file[0] != (byte)'L' || file[1] != (byte)'2' || file[2] != (byte)'I' || file[3] != (byte)'G')
            return;
        int version = BitConverter.ToInt32(file, 4);
        int n = BitConverter.ToInt32(file, 8);
        int count = BitConverter.ToInt32(file, 12);
        if (version != 2 || n != IntensityGridSize || count <= 0)
            return;

        int cursor = 16;
        int plane = n * n;
        for (int s = 0; s < count; s++)
        {
            if (cursor + 4 + plane * 2 > file.Length)
                break;
            float hours = BitConverter.ToSingle(file, cursor);
            cursor += 4;
            var intensity = new byte[plane];
            var valid = new byte[plane];
            System.Buffer.BlockCopy(file, cursor, intensity, 0, plane);
            cursor += plane;
            System.Buffer.BlockCopy(file, cursor, valid, 0, plane);
            cursor += plane;
            IntensitySlices.Add(new IntensitySlice { tHours = hours, intensity = intensity, valid = valid });
        }

        IntensitySlices.Sort((x, y) => x.tHours.CompareTo(y.tHours));
        // Debug.Log("[L2EnvWorldLut] Loaded " + IntensitySlices.Count + " intensity slices from " + fileName);
    }

    static void EnsureNeighborGrids()
    {
        for (int i = 0; i < NeighborGrids.Length; i++)
        {
            NeighborIntensityGrid grid = NeighborGrids[i];
            if (grid.tried)
                continue;
            grid.tried = true;
            if (!ReadIntensityGrid(grid.fileName, grid.slices))
                Shader.SetGlobalVector(grid.originId, Vector4.zero);
        }
    }

    static bool ReadIntensityGrid(string fileName, List<IntensitySlice> destination)
    {
        destination.Clear();
        string path = Path.Combine(Application.streamingAssetsPath, "Data/Meta", fileName);
        if (!File.Exists(path))
        {
            // Debug.LogWarning("[L2EnvWorldLut] Intensity grid missing (" + fileName + ")");
            return false;
        }

        byte[] file = File.ReadAllBytes(path);
        if (file.Length < 16 || file[0] != (byte)'L' || file[1] != (byte)'2' || file[2] != (byte)'I' || file[3] != (byte)'G')
            return false;
        int version = BitConverter.ToInt32(file, 4);
        int n = BitConverter.ToInt32(file, 8);
        int count = BitConverter.ToInt32(file, 12);
        if (version != 2 || n != IntensityGridSize || count <= 0)
            return false;

        int cursor = 16;
        int plane = n * n;
        for (int s = 0; s < count; s++)
        {
            if (cursor + 4 + plane * 2 > file.Length)
                break;
            float hours = BitConverter.ToSingle(file, cursor);
            cursor += 4;
            var intensity = new byte[plane];
            var valid = new byte[plane];
            System.Buffer.BlockCopy(file, cursor, intensity, 0, plane);
            cursor += plane;
            System.Buffer.BlockCopy(file, cursor, valid, 0, plane);
            cursor += plane;
            destination.Add(new IntensitySlice { tHours = hours, intensity = intensity, valid = valid });
        }

        destination.Sort((x, y) => x.tHours.CompareTo(y.tHours));
        // Debug.Log("[L2EnvWorldLut] Loaded " + destination.Count + " intensity slices from " + fileName);
        return destination.Count > 0;
    }
}
