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
    static float Intensity01(Vector3 world, Vector4 origin, Texture2D intensityTex)
    {
        float cell = Mathf.Max(origin.z, 1f);
        float worldUuX = world.x * 52.5f;
        float worldUuY = world.z * 52.5f;
        int ix = Mathf.FloorToInt((worldUuY - origin.x) / cell);
        int iy = Mathf.FloorToInt((worldUuX - origin.y) / cell);
        if (intensityTex != null && ix >= 0 && iy >= 0 && ix < intensityTex.width && iy < intensityTex.height)
        {
            return intensityTex.GetPixel(ix, iy).r;
        }

        return Shader.GetGlobalFloat("_L2TerrainIntensity");
    }

    static Vector3 TerrainShaderColor0(Vector3 meshColor, Vector3 ambient, Vector3 hsv, float intensity01)
    {
        Vector3 vtx = meshColor;
        if (Mathf.Max(vtx.x, Mathf.Max(vtx.y, vtx.z)) < 0.001f)
        {
            vtx = Vector3.one;
        }

        float along = Mathf.Clamp01(intensity01);
        Vector3 sunByte = new Vector3(
            Mathf.Floor(hsv.x * 0.5f * along * 255f),
            Mathf.Floor(hsv.y * 0.5f * along * 255f),
            Mathf.Floor(hsv.z * 0.5f * along * 255f));
        Vector3 ambByte = new Vector3(
            Mathf.Floor(Mathf.Clamp01(ambient.x) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(ambient.y) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(ambient.z) * 255f + 0.5f));
        Vector3 ambShift = new Vector3(
            Mathf.Floor(ambByte.x * 0.5f),
            Mathf.Floor(ambByte.y * 0.5f),
            Mathf.Floor(ambByte.z * 0.5f));
        if (Mathf.Min(vtx.x, Mathf.Min(vtx.y, vtx.z)) > 0.98f)
        {
            vtx = Vector3.zero;
        }

        Vector3 baseByte = new Vector3(
            Mathf.Floor(Mathf.Clamp01(vtx.x) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(vtx.y) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(vtx.z) * 255f + 0.5f));
        float scale = Shader.GetGlobalFloat("_L2TerrainColor0Scale");
        if (scale < 0.001f)
        {
            scale = 1f;
        }

        Vector3 color0 = Vector3.Min(baseByte + sunByte + ambShift, Vector3.one * 255f) * (scale / 255f);
        return Vector3.Min(Vector3.Max(color0, Vector3.zero), Vector3.one);
    }
}
