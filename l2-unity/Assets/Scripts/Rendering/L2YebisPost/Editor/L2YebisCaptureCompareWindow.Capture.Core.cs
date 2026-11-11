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
    static int ChannelDelta(float unity, float client)
    {
        int delta = ToByte(unity) - ToByte(client);
        return delta < 0 ? -delta : delta;
    }

    static int ToByte(float value)
    {
        return Mathf.Clamp(Mathf.RoundToInt(value * 255f), 0, 255);
    }

    static void CountChannel(int delta, ref int zero, ref int one, ref int two, ref int more)
    {
        if (delta == 0) zero++;
        else if (delta == 1) one++;
        else if (delta == 2) two++;
        else more++;
    }

    static void AppendUvNote(StringBuilder stats, string folder)
    {
        stats.AppendLine("Unity читает s0, s1 и s3 одним экранным UV 0..1.");
        string path = Path.Combine(folder, "uvs.txt");
        if (!File.Exists(path))
        {
            stats.AppendLine("uvs.txt нет, фактический v3 этого draw не записан.");
            return;
        }
        foreach (string line in File.ReadAllLines(path))
        {
            if (line.StartsWith("vertex="))
            {
                stats.AppendLine(line);
            }
        }
        stats.AppendLine("tex0 и tex1 — UV сцены и блика. tex3 — UV дизера, это не экранный UV.");
    }

    static Texture2D MakeTexture(float[] pixels, int width, int height)
    {
        var texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false, true)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        texture.SetPixelData(pixels, 0);
        texture.Apply(false, false);
        return texture;
    }

    static float[] LoadRaw(string path, Dictionary<string, string> meta, out int width, out int height)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Нет файла " + path);
        }

        width = IntValue(meta, "width");
        height = IntValue(meta, "height");
        int stride = IntValue(meta, "stride");
        string order = Value(meta, "memoryOrder");
        if (order == "unknown" && Value(meta, "format") == "31")
        {
            order = "A2B10G10R10";
        }
        if (width <= 0 || height <= 0 || stride <= 0)
        {
            throw new InvalidOperationException("В meta нет width/height/stride: " + path);
        }

        byte[] file = File.ReadAllBytes(path);
        if (file.Length < (long)stride * height)
        {
            throw new InvalidOperationException("Файл короче stride*height: " + path);
        }

        int bpp = BytesPerPixel(order);
        var pixels = new float[width * height * 4];
        for (int y = 0; y < height; y++)
        {
            int fileY = height - 1 - y;
            int row = fileY * stride;
            for (int x = 0; x < width; x++)
            {
                int dst = (y * width + x) * 4;
                DecodePixel(file, row + x * bpp, order, pixels, dst);
            }
        }
        return pixels;
    }
}
