using System;
using UnityEngine;
using System.Collections.Generic;
using VContainer;

public partial class HitManager
{
    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.zero;
    }

    private static float AngleDeg(Vector3 a, Vector3 b)
    {
        if (a.sqrMagnitude < 0.0001f || b.sqrMagnitude < 0.0001f)
        {
            return -1f;
        }

        return Vector3.Angle(a, b);
    }
}
