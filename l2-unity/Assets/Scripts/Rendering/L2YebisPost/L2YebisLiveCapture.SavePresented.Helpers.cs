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
}
