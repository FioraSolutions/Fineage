using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2DayNightLut
{
    public static Color ApplyPostSky(Color rawSky, float worldHours)
    {
        float dayWeight = PostSkyDayWeight(worldHours);
        Color result = rawSky + PostSkyDayOffset * dayWeight;
        result.r = Mathf.Clamp01(result.r);
        result.g = Mathf.Clamp01(result.g);
        result.b = Mathf.Clamp01(result.b);
        result.a = rawSky.a;
        return result;
    }

    static float PostSkyDayWeight(float worldHours)
    {
        float h = Mathf.Repeat(worldHours, 24f);
        if (h < 6f)
            return 0f;
        if (h < 8f)
            return Smooth01((h - 6f) * 0.5f);
        if (h < 22f)
            return 1f;
        return 1f - Smooth01((h - 22f) * 0.5f);
    }

    static float Smooth01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }

    static float Parse(string s) => float.Parse(s, CultureInfo.InvariantCulture);
}
