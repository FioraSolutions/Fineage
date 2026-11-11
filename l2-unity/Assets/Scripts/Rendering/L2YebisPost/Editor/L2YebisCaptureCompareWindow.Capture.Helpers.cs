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
