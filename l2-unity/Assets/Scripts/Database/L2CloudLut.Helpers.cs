using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static partial class L2CloudLut
{
    public static Sample SampleAt(float worldHours)
    {
        EnsureLoaded();
        if (Samples.Count == 0)
            return new Sample { sheet = SheetFallback, silhouette = SilhouetteFallback };
        if (Samples.Count == 1)
            return Samples[0];

        float t = Mathf.Repeat(worldHours, 24f);
        int i1 = 0;
        while (i1 < Samples.Count && Samples[i1].tHours < t)
            i1++;

        Sample a;
        Sample b;
        float u;
        if (i1 <= 0 || i1 >= Samples.Count)
        {
            a = Samples[Samples.Count - 1];
            b = Samples[0];
            float span = (24f - a.tHours) + b.tHours;
            float along = t >= a.tHours
                ? t - a.tHours
                : (24f - a.tHours) + t;
            u = span > 1e-6f ? along / span : 0f;
        }
        else
        {
            a = Samples[i1 - 1];
            b = Samples[i1];
            float span = b.tHours - a.tHours;
            u = span > 1e-6f ? (t - a.tHours) / span : 0f;
        }

        return new Sample
        {
            tHours = t,
            sheet = Color.Lerp(a.sheet, b.sheet, u),
            silhouette = Color.Lerp(a.silhouette, b.silhouette, u)
        };
    }

    static float Parse(string value) =>
        float.Parse(value, CultureInfo.InvariantCulture);
}
