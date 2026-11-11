using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static partial class L2HazeLut
{
    public static Color SampleAt(float worldHours)
    {
        EnsureLoaded();
        if (Samples.Count == 0)
        {
            return ColorModifierDefault;
        }

        if (Samples.Count == 1)
        {
            return Samples[0].haze;
        }

        float t = worldHours % 24f;
        if (t < 0f)
        {
            t += 24f;
        }

        int i1 = 0;
        while (i1 < Samples.Count && Samples[i1].tHours < t)
        {
            i1++;
        }

        Sample a;
        Sample b;
        float u;
        if (i1 <= 0 || i1 >= Samples.Count)
        {
            a = Samples[Samples.Count - 1];
            b = Samples[0];
            float span = (24f - a.tHours) + b.tHours;
            float along = t >= a.tHours ? (t - a.tHours) : ((24f - a.tHours) + t);
            u = span > 1e-6f ? along / span : 0f;
        }
        else
        {
            a = Samples[i1 - 1];
            b = Samples[i1];
            float span = b.tHours - a.tHours;
            u = span > 1e-6f ? (t - a.tHours) / span : 0f;
        }

        return Color.Lerp(a.haze, b.haze, u);
    }

    static float Parse(string s) => float.Parse(s, CultureInfo.InvariantCulture);
}
