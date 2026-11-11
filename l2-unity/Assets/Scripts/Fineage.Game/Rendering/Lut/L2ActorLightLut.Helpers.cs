using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2ActorLightLut
{
    public static Sample SampleAt(float worldHours)
    {
        if (!IsReady)
            return default;

        float t = worldHours % 24f;
        if (t < 0f)
            t += 24f;

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

        return new Sample
        {
            tHours = t,
            ambient = Color.Lerp(a.ambient, b.ambient, u),
            ambientHalf = Color.Lerp(a.ambientHalf, b.ambientHalf, u),
            sunPlane = Color.Lerp(a.sunPlane, b.sunPlane, u),
            sunBase = Color.Lerp(a.sunBase, b.sunBase, u)
        };
    }

    public static void PushGlobals(float worldHours)
    {
        EnsureLoaded();
        if (!IsReady)
            return;

        Sample sample = SampleAt(worldHours);
        Shader.SetGlobalColor(ActorAmbientHalfId, sample.ambientHalf);
        Shader.SetGlobalColor(HsvActorLightId, sample.sunPlane);
        Shader.SetGlobalColor(HsvActorBaseId, sample.sunBase);
        Shader.SetGlobalFloat(ActorDirectionalScaleId, DirectionalScale);
    }

    static float Parse(string s) => float.Parse(s, CultureInfo.InvariantCulture);
}
