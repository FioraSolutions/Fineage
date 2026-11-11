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
}
