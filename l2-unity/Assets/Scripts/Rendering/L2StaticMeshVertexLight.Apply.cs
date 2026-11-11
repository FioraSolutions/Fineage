using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2StaticMeshVertexLight
{
    public static float WorldRadiusUu(float lightRadius)
    {
        return (lightRadius + 1f) * 25f;
    }

    public static Vector3 ToUnityPosition(Vector3 l2)
    {
        return new Vector3(l2.y, l2.z, l2.x) * UuToMeters;
    }

    public static Vector3 ToUnityDirection(Vector3 l2)
    {
        return new Vector3(l2.y, l2.z, l2.x);
    }

    public static Vector3 RotatorToL2Direction(float pitch, float yaw)
    {
        float pr = pitch * Mathf.PI / 32768f;
        float yr = yaw * Mathf.PI / 32768f;
        float cp = Mathf.Cos(pr);
        return new Vector3(cp * Mathf.Cos(yr), cp * Mathf.Sin(yr), Mathf.Sin(pr));
    }

    public static Color LightPlane(int hue, int saturation, float brightness)
    {
        Vector3 chroma = Chroma(hue, saturation);
        const float storedScale = 0.547547f;
        float v = Mathf.Max(brightness, 0f) / 255f * storedScale;
        return new Color(chroma.x * v, chroma.y * v, chroma.z * v, 1f);
    }
}
