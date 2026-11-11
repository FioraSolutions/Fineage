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
    static float FilmicChannel(float x, float filmicA, float filmicB)
    {
        float e = Mathf.Exp(-filmicA * x);
        float oneMinusBe = 1f - filmicB * e;
        return Mathf.Clamp01((1f - e) * oneMinusBe * oneMinusBe);
    }

    static Vector3 CaptureGamma(Vector3 combined)
    {
        const float floor = 5.96046448e-8f;
        return new Vector3(GammaChannel(combined.x, floor), GammaChannel(combined.y, floor), GammaChannel(combined.z, floor));
    }

    static float GammaChannel(float value, float floor)
    {
        float safe = Mathf.Max(value, floor);
        return Mathf.Clamp01(Mathf.Exp(Mathf.Log(safe) * 0.625f));
    }

    static string Fmt(Vector3 value)
    {
        return value.x.ToString("0.####", CultureInfo.InvariantCulture) + " " +
               value.y.ToString("0.####", CultureInfo.InvariantCulture) + " " +
               value.z.ToString("0.####", CultureInfo.InvariantCulture);
    }
}
