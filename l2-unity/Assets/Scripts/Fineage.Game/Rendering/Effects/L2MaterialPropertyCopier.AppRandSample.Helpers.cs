using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    private static float AppRandFrand(ref uint state)
    {
        return AppRandStep(ref state) / 32767f;
    }

    private static float AppRandFRange(float min, float max, ref uint state)
    {
        return AppRandFrand(ref state) * (min - max) + max;
    }

    private static float AppRandFRangeVectorZ(
        float xMin, float xMax,
        float yMin, float yMax,
        float zMin, float zMax,
        ref uint state)
    {
        float z = AppRandFRange(zMin, zMax, ref state);
        AppRandFRange(yMin, yMax, ref state);
        AppRandFRange(xMin, xMax, ref state);
        return z;
    }

    private static Vector3 AppRandFRangeVector(
        Vector2 xRange,
        Vector2 yRange,
        Vector2 zRange,
        ref uint state)
    {
        float z = AppRandFRange(zRange.x, zRange.y, ref state);
        float y = AppRandFRange(yRange.x, yRange.y, ref state);
        float x = AppRandFRange(xRange.x, xRange.y, ref state);
        return new Vector3(x, y, z);
    }
}
