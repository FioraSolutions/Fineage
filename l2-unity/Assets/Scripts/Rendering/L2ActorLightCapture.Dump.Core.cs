using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static partial class L2ActorLightCapture
{
    static bool Contains(string value, string part)
    {
        return !string.IsNullOrEmpty(value)
            && value.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static Color ToLinear(Color c)
    {
        return new Color(
            Mathf.GammaToLinearSpace(c.r),
            Mathf.GammaToLinearSpace(c.g),
            Mathf.GammaToLinearSpace(c.b),
            c.a);
    }

    static int Byte(float channel)
    {
        return Mathf.Clamp(Mathf.RoundToInt(channel * 255f), 0, 255);
    }

    static string Num(float v)
    {
        return v.ToString("0.000000000", CultureInfo.InvariantCulture);
    }
}
