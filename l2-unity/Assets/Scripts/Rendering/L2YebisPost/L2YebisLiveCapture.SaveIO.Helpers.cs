using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public static partial class L2YebisLiveCapture
{
    static void WriteBands(string path, string title, int width, int height, int pixels, int above, int near, Band[] bands, bool output, int atMax = 0)
    {
        var text = new StringBuilder();
        text.AppendLine(title + " " + width + "x" + height);
        if (!output)
        {
            text.AppendLine("пикселей с каналом > 1: " + above + " из " + pixels);
            text.AppendLine("пикселей почти чёрных, пик < 1/255: " + near);
        }
        else
        {
            text.AppendLine("пикселей с каналом >= 254: " + atMax + " из " + pixels);
        }
        text.AppendLine("Полосы кадра, не одни и те же камни. y растёт снизу вверх.");
        string[] names = { "низ", "середина", "верх" };
        for (int i = 0; i < 3; i++)
        {
            text.AppendLine(names[i] + " медиана RGB " + bands[i].MedianText() + "  пикселей " + bands[i].Count);
        }
        File.WriteAllText(path, text.ToString(), Encoding.UTF8);
    }

    static void WriteFloat(byte[] file, int offset, float value)
    {
        byte[] bits = BitConverter.GetBytes(value);
        file[offset] = bits[0];
        file[offset + 1] = bits[1];
        file[offset + 2] = bits[2];
        file[offset + 3] = bits[3];
    }

    static byte ToByte(float value)
    {
        int n = Mathf.RoundToInt(Mathf.Clamp01(value) * 255f);
        return (byte)n;
    }
}
