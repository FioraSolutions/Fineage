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
    static void WriteMeta(string folder, string name, int width, int height, int bpp, string formatName, string order, string where)
    {
        var text = new StringBuilder();
        text.AppendLine("width=" + width);
        text.AppendLine("height=" + height);
        text.AppendLine("formatName=" + formatName);
        text.AppendLine("memoryOrder=" + order);
        text.AppendLine("stride=" + (width * bpp));
        text.AppendLine("size=" + (width * height * bpp));
        text.AppendLine("where=" + where);
        text.AppendLine("present=1");
        text.AppendLine("origin=top-row-first");
        File.WriteAllText(Path.Combine(folder, name + ".meta.txt"), text.ToString(), Encoding.UTF8);
    }

    static void WritePng(string folder, string name, Color32[] pixels, int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
        tex.SetPixelData(pixels, 0);
        tex.Apply(false, false);
        File.WriteAllBytes(Path.Combine(folder, name), tex.EncodeToPNG());
        UnityEngine.Object.Destroy(tex);
    }

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

    struct Band
    {
        public float[] R;
        public float[] G;
        public float[] B;
        public int Count;

        public static Band Create(int capacity)
        {
            return new Band
            {
                R = new float[capacity],
                G = new float[capacity],
                B = new float[capacity],
                Count = 0
            };
        }

        public void Add(float r, float g, float b)
        {
            if (Count >= R.Length)
            {
                return;
            }
            R[Count] = r;
            G[Count] = g;
            B[Count] = b;
            Count++;
        }

        public string MedianText()
        {
            return Median(R).ToString("0.####", CultureInfo.InvariantCulture) + " " +
                   Median(G).ToString("0.####", CultureInfo.InvariantCulture) + " " +
                   Median(B).ToString("0.####", CultureInfo.InvariantCulture);
        }

        float Median(float[] values)
        {
            if (Count <= 0)
            {
                return 0f;
            }
            var copy = new float[Count];
            Array.Copy(values, copy, Count);
            Array.Sort(copy);
            return copy[Count / 2];
        }
    }
}
