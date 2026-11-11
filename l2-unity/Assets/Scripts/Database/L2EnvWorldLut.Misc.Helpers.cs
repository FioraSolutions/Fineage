using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    static void UploadNeighborGrids(float hours, byte fill)
    {
        int n = IntensityGridSize;
        var pixels = new byte[n * n * 4];
        for (int g = 0; g < NeighborGrids.Length; g++)
        {
            NeighborIntensityGrid grid = NeighborGrids[g];
            if (grid.slices.Count == 0)
            {
                Shader.SetGlobalVector(grid.originId, Vector4.zero);
                continue;
            }

            int a;
            int b;
            float u;
            PickIntensitySlices(grid.slices, hours, out a, out b, out u);
            if (grid.tex != null && a == grid.sliceA && b == grid.sliceB && fill == grid.fill)
                continue;

            if (a < 0)
            {
                for (int i = 0; i < n * n; i++)
                {
                    pixels[i * 4] = fill;
                    pixels[i * 4 + 1] = 0;
                    pixels[i * 4 + 3] = 255;
                }
            }
            else
            {
                IntensitySlice sa = grid.slices[a];
                IntensitySlice sb = grid.slices[b];
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

            if (grid.tex == null)
            {
                grid.tex = new Texture2D(n, n, TextureFormat.RGBA32, false, true)
                {
                    name = grid.fileName,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point
                };
                grid.tex.hideFlags = HideFlags.HideAndDontSave;
            }

            grid.tex.SetPixelData(pixels, 0);
            grid.tex.Apply(false, false);
            Shader.SetGlobalTexture(grid.textureId, grid.tex);
            Shader.SetGlobalVector(grid.originId, grid.origin);
            grid.sliceA = a;
            grid.sliceB = b;
            grid.fill = fill;
        }
    }

    static void PickIntensitySlices(List<IntensitySlice> slices, float hours, out int a, out int b, out float u)
    {
        a = -1;
        b = -1;
        u = 0f;
        int count = slices.Count;
        if (count == 0)
            return;
        if (count == 1)
        {
            a = b = 0;
            return;
        }

        // Slot centres are 1.5, 4.5, ... 22.5. Hour 1.0 is the pair 22.5 → 1.5,
        // alpha 5/6, the same wrap as UTerrainSector::UpdateShadow. Clamping to
        // the 1.5 slice alone leaves the lit path byte at 156 instead of 130.
        float first = slices[0].tHours;
        float last = slices[count - 1].tHours;
        if (hours < first || hours > last)
        {
            a = count - 1;
            b = 0;
            float timeA = last;
            float timeB = first + 24f;
            float time = hours < first ? hours + 24f : hours;
            float width = timeB - timeA;
            u = width > 1e-6f ? (time - timeA) / width : 0f;
            return;
        }

        int i1 = 1;
        while (i1 < count && slices[i1].tHours < hours)
            i1++;
        a = i1 - 1;
        b = i1;
        float span = slices[b].tHours - slices[a].tHours;
        u = span > 1e-6f ? (hours - slices[a].tHours) / span : 0f;
    }
}
