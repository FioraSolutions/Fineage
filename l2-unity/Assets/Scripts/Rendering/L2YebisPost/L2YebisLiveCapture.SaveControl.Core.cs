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
}
