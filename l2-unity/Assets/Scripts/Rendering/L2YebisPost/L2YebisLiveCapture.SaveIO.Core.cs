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
}
