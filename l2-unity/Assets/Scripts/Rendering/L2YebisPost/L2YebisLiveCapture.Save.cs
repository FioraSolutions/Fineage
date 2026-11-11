using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public partial class L2YebisLiveCapture
{
    static void SaveControl(string folder, RenderTexture rt)
    {
        if (rt == null)
        {
            // Debug.LogWarning("[Yebis] control rt null");
            return;
        }

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, true);
        tex.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0, false);
        tex.Apply(false, false);
        RenderTexture.active = previous;
        Color32 pixel = tex.GetPixel(0, 0);
        int peak = 0;
        Color32[] pixels = tex.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            peak = Math.Max(peak, Math.Max(pixels[i].r, Math.Max(pixels[i].g, pixels[i].b)));
        }

        File.WriteAllText(
            Path.Combine(folder, "control.txt"),
            "format=" + rt.graphicsFormat + "\n" +
            "created=" + rt.IsCreated() + "\n" +
            "size=" + rt.width + "x" + rt.height + "\n" +
            "expected=0.8 0.2 0.05\n" +
            "pixel0=" + pixel.r + " " + pixel.g + " " + pixel.b + "\n" +
            "peak=" + peak + "\n",
            Encoding.UTF8);
        // Debug.Log("[Yebis] control " + rt.graphicsFormat + " " + rt.width + "x" + rt.height + " created=" + rt.IsCreated() + " pixel0=" + pixel.r + "," + pixel.g + "," + pixel.b + " peak=" + peak);
        UnityEngine.Object.Destroy(tex);
    }

    static void SaveHdr(string folder, RenderTexture rt, string name, string where, string title)
    {
        if (rt == null)
        {
            // Debug.LogWarning("[Yebis] " + name + " rt null");
            return;
        }

        AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(rt, 0, GraphicsFormat.R32G32B32A32_SFloat);
        request.WaitForCompletion();
        if (request.hasError)
        {
            // Debug.LogWarning("[Yebis] " + name + " float readback failed " + rt.graphicsFormat);
            return;
        }

        NativeArray<float> data = request.GetData<float>();
        int width = rt.width;
        int height = rt.height;
        int count = width * height;
        if (data.Length < count * 4)
        {
            // Debug.LogWarning("[Yebis] " + name + " float buffer " + data.Length);
            return;
        }

        var file = new byte[count * 16];
        var preview = new Color32[count];
        float minR = float.MaxValue, minG = float.MaxValue, minB = float.MaxValue;
        float maxR = float.MinValue, maxG = float.MinValue, maxB = float.MinValue;
        int above = 0;
        int near = 0;
        ulong hash = 14695981039346656037ul;
        int bandCap = (height / 3 + 1) * width;
        var bands = new Band[3];
        for (int i = 0; i < 3; i++)
        {
            bands[i] = Band.Create(bandCap);
        }

        for (int y = 0; y < height; y++)
        {
            int fileY = height - 1 - y;
            int band = y < height / 3 ? 0 : (y < 2 * height / 3 ? 1 : 2);
            for (int x = 0; x < width; x++)
            {
                int src = (y * width + x) * 4;
                float r = data[src];
                float g = data[src + 1];
                float b = data[src + 2];
                float a = data[src + 3];
                int dst = (fileY * width + x) * 4;
                WriteFloat(file, dst * 4, r);
                WriteFloat(file, (dst + 1) * 4, g);
                WriteFloat(file, (dst + 2) * 4, b);
                WriteFloat(file, (dst + 3) * 4, a);
                HashFloat(ref hash, r);
                HashFloat(ref hash, g);
                HashFloat(ref hash, b);
                minR = Mathf.Min(minR, r);
                minG = Mathf.Min(minG, g);
                minB = Mathf.Min(minB, b);
                maxR = Mathf.Max(maxR, r);
                maxG = Mathf.Max(maxG, g);
                maxB = Mathf.Max(maxB, b);
                float hi = Mathf.Max(r, Mathf.Max(g, b));
                if (hi > 1.001f)
                {
                    above++;
                }
                if (hi < 1f / 255f)
                {
                    near++;
                }
                preview[src / 4] = new Color32(ToByte(r), ToByte(g), ToByte(b), 255);
                bands[band].Add(r, g, b);
            }
        }

        File.WriteAllBytes(Path.Combine(folder, name + ".raw"), file);
        WriteMeta(folder, name, width, height, 16, rt.graphicsFormat.ToString(), "R32G32B32A32F", where);
        WritePng(folder, name + ".png", preview, width, height);
        var text = new StringBuilder();
        text.AppendLine(title + " " + width + "x" + height);
        text.AppendLine("sceneFormat=" + rt.graphicsFormat);
        text.AppendLine("created=" + rt.IsCreated());
        text.AppendLine("min RGB " + minR.ToString("0.####", CultureInfo.InvariantCulture) + " " + minG.ToString("0.####", CultureInfo.InvariantCulture) + " " + minB.ToString("0.####", CultureInfo.InvariantCulture));
        text.AppendLine("max RGB " + maxR.ToString("0.####", CultureInfo.InvariantCulture) + " " + maxG.ToString("0.####", CultureInfo.InvariantCulture) + " " + maxB.ToString("0.####", CultureInfo.InvariantCulture));
        text.AppendLine("канал > 1: " + above + " из " + count);
        text.AppendLine("пик < 1/255: " + near);
        text.AppendLine("hash=" + hash.ToString("X16"));
        text.AppendLine("png — превью, значения выше 1 обрезаны к 255. Числа в raw.");
        text.AppendLine("y в массиве чтения снизу вверх. Файл raw: строка 0 сверху.");
        int y0 = height / 3;
        int y1 = 2 * height / 3;
        text.AppendLine("низ y=[" + 0 + "," + y0 + ") медиана " + bands[0].MedianText());
        text.AppendLine("середина y=[" + y0 + "," + y1 + ") медиана " + bands[1].MedianText());
        text.AppendLine("верх y=[" + y1 + "," + height + ") медиана " + bands[2].MedianText());
        string statsName = name == "input" ? "input_stats.txt" : "output_stats.txt";
        File.WriteAllText(Path.Combine(folder, statsName), text.ToString(), Encoding.UTF8);
        // Debug.Log("[Yebis] " + name + " " + rt.graphicsFormat + " max=" + maxR.ToString("0.###") + "," + maxG.ToString("0.###") + "," + maxB.ToString("0.###") + " above1=" + above + " hash=" + hash.ToString("X16"));
    }

    static void HashFloat(ref ulong hash, float value)
    {
        byte[] bits = BitConverter.GetBytes(value);
        for (int i = 0; i < 4; i++)
        {
            hash ^= bits[i];
            hash *= 1099511628211ul;
        }
    }

    static void SavePresented(string folder, RenderTexture rt, string name, string where, string title)
    {
        if (rt == null)
        {
            // Debug.LogWarning("[Yebis] " + name + " rt null");
            return;
        }

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, true);
        tex.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0, false);
        tex.Apply(false, false);
        RenderTexture.active = previous;

        Color32[] pixels = tex.GetPixels32();
        int width = rt.width;
        int height = rt.height;
        int count = width * height;
        var file = new byte[count * 4];
        int peak = 0;
        int near = 0;
        int atMax = 0;
        ulong hash = 14695981039346656037ul;
        int bandCap = (height / 3 + 1) * width;
        var bands = new Band[3];
        for (int i = 0; i < 3; i++)
        {
            bands[i] = Band.Create(bandCap);
        }

        for (int y = 0; y < height; y++)
        {
            int fileY = height - 1 - y;
            int band = y < height / 3 ? 0 : (y < 2 * height / 3 ? 1 : 2);
            for (int x = 0; x < width; x++)
            {
                Color32 c = pixels[y * width + x];
                int dst = (fileY * width + x) * 4;
                file[dst] = c.r;
                file[dst + 1] = c.g;
                file[dst + 2] = c.b;
                file[dst + 3] = c.a;
                hash ^= c.r;
                hash *= 1099511628211ul;
                hash ^= c.g;
                hash *= 1099511628211ul;
                hash ^= c.b;
                hash *= 1099511628211ul;
                peak = Math.Max(peak, Math.Max(c.r, Math.Max(c.g, c.b)));
                float r = c.r / 255f;
                float g = c.g / 255f;
                float b = c.b / 255f;
                float hi = Mathf.Max(r, Mathf.Max(g, b));
                if (hi < 1f / 255f)
                {
                    near++;
                }
                if (c.r >= 254 || c.g >= 254 || c.b >= 254)
                {
                    atMax++;
                }
                bands[band].Add(r, g, b);
            }
        }

        File.WriteAllBytes(Path.Combine(folder, name + ".raw"), file);
        WriteMeta(folder, name, width, height, 4, "R8G8B8A8", "R8G8B8A8", where);
        File.WriteAllBytes(Path.Combine(folder, name + ".png"), tex.EncodeToPNG());
        UnityEngine.Object.Destroy(tex);
        string statsName = name == "input" ? "input_stats.txt" : "output_stats.txt";
        WriteBands(Path.Combine(folder, statsName), title, width, height, count, 0, near, bands, name != "input", atMax);
        File.AppendAllText(
            Path.Combine(folder, statsName),
            "hash=" + hash.ToString("X16") + "\n" +
            "cameraFormat=R8G8B8A8_UNorm. Канал выше 1 в этом входе невозможен: текстура камеры 8-битная.\n" +
            "Если min и max равны 1 0 1, копия draw не легла поверх пурпурной метки.\n",
            Encoding.UTF8);
        // Debug.Log("[Yebis] " + name + " " + rt.graphicsFormat + " " + width + "x" + height + " peak=" + peak + " hash=" + hash.ToString("X16"));
    }

    public static void SaveFloat(string folder, AsyncGPUReadbackRequest request, int width, int height)
    {
        if (request.hasError || width <= 0 || height <= 0)
        {
            // Debug.LogWarning("[Yebis] live input readback failed");
            return;
        }

        NativeArray<float> data = request.GetData<float>();
        int pixels = width * height;
        if (data.Length < pixels * 4)
        {
            // Debug.LogWarning("[Yebis] live input short " + data.Length);
            return;
        }

        var file = new byte[pixels * 16];
        var preview = new Color32[pixels];
        int above = 0;
        int near = 0;
        int bandCap = (height / 3 + 1) * width;
        var bands = new Band[3];
        for (int i = 0; i < 3; i++)
        {
            bands[i] = Band.Create(bandCap);
        }

        for (int y = 0; y < height; y++)
        {
            int fileY = height - 1 - y;
            int band = y < height / 3 ? 0 : (y < 2 * height / 3 ? 1 : 2);
            for (int x = 0; x < width; x++)
            {
                int src = (y * width + x) * 4;
                float r = data[src];
                float g = data[src + 1];
                float b = data[src + 2];
                float a = data[src + 3];
                int dst = (fileY * width + x) * 4;
                WriteFloat(file, dst * 4, r);
                WriteFloat(file, (dst + 1) * 4, g);
                WriteFloat(file, (dst + 2) * 4, b);
                WriteFloat(file, (dst + 3) * 4, a);
                preview[src / 4] = new Color32(ToByte(r), ToByte(g), ToByte(b), 255);
                float peak = Mathf.Max(r, Mathf.Max(g, b));
                if (peak > 1.001f)
                {
                    above++;
                }
                if (peak < 1f / 255f)
                {
                    near++;
                }
                bands[band].Add(r, g, b);
            }
        }

        File.WriteAllBytes(Path.Combine(folder, "input.raw"), file);
        WriteMeta(folder, "input", width, height, 16, "R32G32B32A32F", "R32G32B32A32F", "unity-s0-before-combine");
        WritePng(folder, "input.png", preview, width, height);
        WriteBands(Path.Combine(folder, "input_stats.txt"), "Вход combine, сырые числа", width, height, pixels, above, near, bands, false);
    }

    public static void SaveColor8(string folder, AsyncGPUReadbackRequest request, int width, int height)
    {
        if (request.hasError || width <= 0 || height <= 0)
        {
            // Debug.LogWarning("[Yebis] live output readback failed");
            return;
        }

        NativeArray<byte> data = request.GetData<byte>();
        int pixels = width * height;
        int stride = data.Length / Mathf.Max(pixels, 1);
        if (stride < 4)
        {
            // Debug.LogWarning("[Yebis] live output stride " + stride);
            return;
        }

        var file = new byte[pixels * 4];
        var preview = new Color32[pixels];
        int atMax = 0;
        int bandCap = (height / 3 + 1) * width;
        var bands = new Band[3];
        for (int i = 0; i < 3; i++)
        {
            bands[i] = Band.Create(bandCap);
        }

        for (int y = 0; y < height; y++)
        {
            int fileY = height - 1 - y;
            int band = y < height / 3 ? 0 : (y < 2 * height / 3 ? 1 : 2);
            for (int x = 0; x < width; x++)
            {
                int src = (y * width + x) * stride;
                byte r = data[src];
                byte g = data[src + 1];
                byte b = data[src + 2];
                byte a = data[src + 3];
                int dst = (fileY * width + x) * 4;
                file[dst] = r;
                file[dst + 1] = g;
                file[dst + 2] = b;
                file[dst + 3] = a;
                preview[y * width + x] = new Color32(r, g, b, 255);
                if (r >= 254 || g >= 254 || b >= 254)
                {
                    atMax++;
                }
                bands[band].Add(r / 255f, g / 255f, b / 255f);
            }
        }

        File.WriteAllBytes(Path.Combine(folder, "output.raw"), file);
        WriteMeta(folder, "output", width, height, 4, "R8G8B8A8", "R8G8B8A8", "unity-after-combine");
        WritePng(folder, "output.png", preview, width, height);
        WriteBands(Path.Combine(folder, "output_stats.txt"), "Выход combine, 8-битный", width, height, pixels, 0, 0, bands, true, atMax);
        // Debug.Log("[Yebis] live capture written " + folder);
    }

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
