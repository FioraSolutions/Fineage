using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static string Num(float v)
    {
        return v.ToString("0.000", CultureInfo.InvariantCulture);
    }

    static string Vec(Color c)
    {
        return Num(c.r) + " " + Num(c.g) + " " + Num(c.b);
    }

    static string Vec(Vector3 v)
    {
        return Num(v.x) + " " + Num(v.y) + " " + Num(v.z);
    }
}
