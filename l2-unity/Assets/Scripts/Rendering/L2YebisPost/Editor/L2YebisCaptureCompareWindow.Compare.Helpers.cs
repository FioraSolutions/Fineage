using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public sealed partial class L2YebisCaptureCompareWindow : EditorWindow
{
    static bool TrySceneToFloat(out string folder)
    {
        folder = null;
        if (!Directory.Exists(GameCaptures))
        {
            return false;
        }

        string[] stamps = Directory.GetDirectories(GameCaptures);
        Array.Sort(stamps, StringComparer.Ordinal);
        for (int i = stamps.Length - 1; i >= 0; i--)
        {
            if (string.Equals(Path.GetFileName(stamps[i]), "unity_live", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string[] draws = Directory.GetDirectories(stamps[i]);
            Array.Sort(draws, StringComparer.Ordinal);
            for (int d = 0; d < draws.Length; d++)
            {
                string statePath = Path.Combine(draws[d], "state.txt");
                if (!File.Exists(statePath) || !File.Exists(Path.Combine(draws[d], "input.raw")) || !File.Exists(Path.Combine(draws[d], "output.raw")))
                {
                    continue;
                }

                if (File.ReadAllText(statePath).Contains("role=scene-to-float"))
                {
                    folder = draws[d];
                    return true;
                }
            }
        }

        return false;
    }

    static float SceneToFloat(float scene)
    {
        const float c0x = -1.96000004f;
        const float c0w = 5f;
        float r1 = scene + c0x;
        float r2 = scene;
        r1 = r1 * r2 + 1f;
        r2 = Mathf.Sqrt(r1);
        return (scene + r2 - 1f) * c0w;
    }

    static void AppendFolderBands(StringBuilder text, string title, string folder, string name)
    {
        Dictionary<string, string> meta = ReadMeta(Path.Combine(folder, name + ".meta.txt"));
        float[] pixels = LoadRaw(Path.Combine(folder, name + ".raw"), meta, out int width, out int height);
        AppendSourceBands(text, title, pixels, width, height);
    }

    static void AppendSourceBands(StringBuilder stats, string title, float[] pixels, int width, int height)
    {
        int above = 0;
        int near = 0;
        int atMax = 0;
        int count = width * height;
        var bands = new System.Collections.Generic.List<float>[3, 3];
        for (int b = 0; b < 3; b++)
        {
            for (int c = 0; c < 3; c++)
            {
                bands[b, c] = new System.Collections.Generic.List<float>(count / 3 + 8);
            }
        }

        for (int y = 0; y < height; y++)
        {
            int band = y < height / 3 ? 0 : (y < 2 * height / 3 ? 1 : 2);
            for (int x = 0; x < width; x++)
            {
                int i = (y * width + x) * 4;
                float r = pixels[i];
                float g = pixels[i + 1];
                float b = pixels[i + 2];
                float peak = Mathf.Max(r, Mathf.Max(g, b));
                if (peak > 1.001f)
                {
                    above++;
                }
                if (peak < 1f / 255f)
                {
                    near++;
                }
                if (ToByte(r) >= 254 || ToByte(g) >= 254 || ToByte(b) >= 254)
                {
                    atMax++;
                }
                bands[band, 0].Add(r);
                bands[band, 1].Add(g);
                bands[band, 2].Add(b);
            }
        }

        stats.AppendLine(title + " " + width + "x" + height);
        stats.AppendLine("канал > 1: " + above + "   пик < 1/255: " + near + "   канал >= 254: " + atMax + "   из " + count);
        string[] names = { "низ", "середина", "верх" };
        for (int b = 0; b < 3; b++)
        {
            stats.AppendLine(names[b] + " медиана RGB " +
                Median(bands[b, 0]).ToString("0.####") + " " +
                Median(bands[b, 1]).ToString("0.####") + " " +
                Median(bands[b, 2]).ToString("0.####"));
        }
    }

    static float[] BuildPairDelta(float[] withGlare, float[] blackGlare, int width, int height, StringBuilder stats)
    {
        var error = new float[width * height * 4];
        int pixels0 = 0, pixels1 = 0, pixels2 = 0, pixelsMore = 0;
        int[] ge2 = { 0, 0, 0 };
        int[] ge7 = { 0, 0, 0 };
        int[] bandCount = { 0, 0, 0 };
        for (int y = 0; y < height; y++)
        {
            int band = y < height / 3 ? 0 : (y < 2 * height / 3 ? 1 : 2);
            for (int x = 0; x < width; x++)
            {
                int i = (y * width + x) * 4;
                int dr = Math.Abs(ToByte(withGlare[i]) - ToByte(blackGlare[i]));
                int dg = Math.Abs(ToByte(withGlare[i + 1]) - ToByte(blackGlare[i + 1]));
                int db = Math.Abs(ToByte(withGlare[i + 2]) - ToByte(blackGlare[i + 2]));
                int worst = Math.Max(dr, Math.Max(dg, db));
                if (worst == 0) pixels0++;
                else if (worst == 1) pixels1++;
                else if (worst == 2) pixels2++;
                else pixelsMore++;
                if (worst >= 2) ge2[band]++;
                if (worst >= 7) ge7[band]++;
                bandCount[band]++;
                float shown = worst >= 2 ? 1f : (worst == 1 ? 0.25f : 0f);
                error[i] = shown;
                error[i + 1] = shown;
                error[i + 2] = shown;
                error[i + 3] = 1f;
            }
        }

        stats.AppendLine("Вклад s1, клиентский s0, уровни A минус чёрный s1:");
        stats.AppendLine("пиксели  0: " + pixels0 + "   1: " + pixels1 + "   2: " + pixels2 + "   3+: " + pixelsMore);
        string[] names = { "низ", "середина", "верх" };
        for (int b = 0; b < 3; b++)
        {
            stats.AppendLine(names[b] + "  разница >= 2: " + ge2[b] + "   >= 7: " + ge7[b] + "   из " + bandCount[b]);
        }
        return error;
    }

    static float Median(System.Collections.Generic.List<float> values)
    {
        if (values == null || values.Count == 0)
        {
            return 0f;
        }
        values.Sort();
        return values[values.Count / 2];
    }

    static void DestroyPreview(ref Texture2D texture)
    {
        if (texture != null)
        {
            DestroyImmediate(texture);
            texture = null;
        }
    }
}
