using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Official terrain-layer color UV. Layer Scale is the ToWorld basis: UU covered
/// by one repeat at UScale 1. UScale enlarges that repeat (the same direction the
/// previous shader used when it divided world/128 by UScale).
/// _Layer_N_UV.xy is that repeat in UU. .zw is UPan/VPan in UV units.
/// _Layer_N_Xform.x is TextureRotation (65536 = one turn). .y is the map axis
/// (0 XY, 1 XZ, 2 YZ). L2_TerrainLayerUv in L2TerrainCommon.hlsl matches Sample.
/// </summary>
public static class L2TerrainLayerUv
{
    public const int ShaderLayerCap = 23;
    public const float Turn = 65536f;

    public static bool TryRepeat(
        float scaleX,
        float scaleY,
        float uScale,
        float vScale,
        float uPan,
        float vPan,
        string layerName,
        List<string> errors,
        out Vector4 repeat)
    {
        repeat = Vector4.zero;
        string who = string.IsNullOrEmpty(layerName) ? "(unnamed)" : layerName;
        if (scaleX <= 1e-4f || scaleY <= 1e-4f)
        {
            errors?.Add("[TerrainUv] layer " + who + " Scale missing");
            return false;
        }
        if (uScale <= 1e-4f || vScale <= 1e-4f)
        {
            errors?.Add("[TerrainUv] layer " + who + " UScale/VScale missing");
            return false;
        }
        repeat = new Vector4(scaleX * uScale, scaleY * vScale, uPan, vPan);
        return true;
    }

    /// <summary>
    /// ueX/ueY/ueZ are official world UU. Axis 0 projects XY, 1 XZ, 2 YZ.
    /// </summary>
    public static Vector2 Sample(float ueX, float ueY, float ueZ, Vector4 repeat, float texRotUu, int axis)
    {
        float su = repeat.x;
        float sv = repeat.y;
        if (su < 1e-4f)
            su = 1e-4f;
        if (sv < 1e-4f)
            sv = 1e-4f;
        float u;
        float v;
        if (axis >= 2)
        {
            u = ueY / su;
            v = ueZ / sv;
        }
        else if (axis == 1)
        {
            u = ueX / su;
            v = ueZ / sv;
        }
        else
        {
            u = ueX / su;
            v = ueY / sv;
        }
        u += repeat.z;
        v += repeat.w;
        if (Mathf.Abs(texRotUu) > 0.5f)
        {
            float a = texRotUu * (Mathf.PI * 2f / Turn);
            float c = Mathf.Cos(a);
            float s = Mathf.Sin(a);
            float ru = u * c - v * s;
            float rv = u * s + v * c;
            u = ru;
            v = rv;
        }
        return new Vector2(u, v);
    }
}
