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
    static void AppendPoint(
        StringBuilder text,
        string tag,
        Vector3 world,
        Vector3 meshColor,
        Vector4 origin,
        Vector3 ambient,
        Vector3 hsv,
        Texture2D intensityTex)
    {
        float cell = Mathf.Max(origin.z, 1f);
        float grid = Mathf.Max(origin.w, 1f);
        float worldUuX = world.x * 52.5f;
        float worldUuY = world.z * 52.5f;
        float gx = (worldUuY - origin.x) / cell;
        float gy = (worldUuX - origin.y) / cell;
        int ix = Mathf.FloorToInt(gx);
        int iy = Mathf.FloorToInt(gy);
        int sectorX = ix >= 0 ? (ix / 16) * 16 : -(((-ix) + 15) / 16) * 16;
        int sectorY = iy >= 0 ? (iy / 16) * 16 : -(((-iy) + 15) / 16) * 16;
        int localX = ix - sectorX;
        int localY = iy - sectorY;
        int intensity = -1;
        int valid = 0;
        if (intensityTex != null && ix >= 0 && iy >= 0 && ix < intensityTex.width && iy < intensityTex.height)
        {
            Color32 pixel = intensityTex.GetPixel(ix, iy);
            intensity = pixel.r;
            valid = pixel.g;
        }

        float intensity01 = intensity >= 0 ? intensity / 255f : Shader.GetGlobalFloat("_L2TerrainIntensity");
        Vector3 color0 = TerrainShaderColor0(meshColor, ambient, hsv, intensity01);
        text.AppendLine(
            tag + "_cell gx=" + ix + " gy=" + iy +
            " sector=" + sectorX + "," + sectorY +
            " local=" + localX + "," + localY +
            " intensity_byte=" + intensity +
            " valid=" + valid +
            " color0 " + Fmt(color0));
    }
}
