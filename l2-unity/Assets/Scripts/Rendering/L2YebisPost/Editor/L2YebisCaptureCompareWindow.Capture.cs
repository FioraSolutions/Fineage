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

    static void DecodePixel(byte[] file, int offset, string order, float[] dst, int dstOffset)
    {
        switch (order)
        {
            case "B8G8R8A8":
                dst[dstOffset] = file[offset + 2] / 255f;
                dst[dstOffset + 1] = file[offset + 1] / 255f;
                dst[dstOffset + 2] = file[offset] / 255f;
                dst[dstOffset + 3] = file[offset + 3] / 255f;
                break;
            case "R8G8B8A8":
                dst[dstOffset] = file[offset] / 255f;
                dst[dstOffset + 1] = file[offset + 1] / 255f;
                dst[dstOffset + 2] = file[offset + 2] / 255f;
                dst[dstOffset + 3] = file[offset + 3] / 255f;
                break;
            case "A2B10G10R10":
            {
                uint packed = BitConverter.ToUInt32(file, offset);
                dst[dstOffset] = (packed & 0x3FFu) / 1023f;
                dst[dstOffset + 1] = ((packed >> 10) & 0x3FFu) / 1023f;
                dst[dstOffset + 2] = ((packed >> 20) & 0x3FFu) / 1023f;
                dst[dstOffset + 3] = ((packed >> 30) & 0x3u) / 3f;
                break;
            }
            case "R16G16B16A16F":
                dst[dstOffset] = HalfToFloat(BitConverter.ToUInt16(file, offset));
                dst[dstOffset + 1] = HalfToFloat(BitConverter.ToUInt16(file, offset + 2));
                dst[dstOffset + 2] = HalfToFloat(BitConverter.ToUInt16(file, offset + 4));
                dst[dstOffset + 3] = HalfToFloat(BitConverter.ToUInt16(file, offset + 6));
                break;
            case "R16G16B16A16":
                dst[dstOffset] = BitConverter.ToUInt16(file, offset) / 65535f;
                dst[dstOffset + 1] = BitConverter.ToUInt16(file, offset + 2) / 65535f;
                dst[dstOffset + 2] = BitConverter.ToUInt16(file, offset + 4) / 65535f;
                dst[dstOffset + 3] = BitConverter.ToUInt16(file, offset + 6) / 65535f;
                break;
            case "R32G32B32A32F":
                dst[dstOffset] = BitConverter.ToSingle(file, offset);
                dst[dstOffset + 1] = BitConverter.ToSingle(file, offset + 4);
                dst[dstOffset + 2] = BitConverter.ToSingle(file, offset + 8);
                dst[dstOffset + 3] = BitConverter.ToSingle(file, offset + 12);
                break;
            default:
                throw new InvalidOperationException("Неизвестный memoryOrder " + order);
        }
    }

    static int BytesPerPixel(string order)
    {
        switch (order)
        {
            case "B8G8R8A8":
            case "R8G8B8A8":
            case "A2B10G10R10":
                return 4;
            case "R16G16B16A16F":
            case "R16G16B16A16":
                return 8;
            case "R32G32B32A32F":
                return 16;
            default:
                throw new InvalidOperationException("Неизвестный memoryOrder " + order);
        }
    }

    static float HalfToFloat(ushort value)
    {
        uint sign = (uint)(value & 0x8000) << 16;
        uint exp = (uint)((value >> 10) & 0x1F);
        uint mant = (uint)(value & 0x3FF);
        uint bits;
        if (exp == 0)
        {
            if (mant == 0)
            {
                bits = sign;
            }
            else
            {
                exp = 1;
                while ((mant & 0x400) == 0)
                {
                    mant <<= 1;
                    exp--;
                }
                mant &= 0x3FF;
                bits = sign | ((exp + (127 - 15)) << 23) | (mant << 13);
            }
        }
        else if (exp == 31)
        {
            bits = sign | 0x7F800000u | (mant << 13);
        }
        else
        {
            bits = sign | ((exp + (127 - 15)) << 23) | (mant << 13);
        }
        return BitConverter.Int32BitsToSingle(unchecked((int)bits));
    }

    static string Value(Dictionary<string, string> map, string key)
    {
        return map != null && map.TryGetValue(key, out string value) ? value : "";
    }

    static int IntValue(Dictionary<string, string> map, string key)
    {
        return int.TryParse(Value(map, key), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : 0;
    }

    static Vector4 VectorOf(Dictionary<string, string> map, string key, Vector4 fallback)
    {
        string text = Value(map, key);
        if (string.IsNullOrEmpty(text))
        {
            return fallback;
        }
        string[] parts = text.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4)
        {
            return fallback;
        }
        return new Vector4(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]), ParseFloat(parts[3]));
    }

    static float ParseFloat(string text)
    {
        return float.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
    }
}
