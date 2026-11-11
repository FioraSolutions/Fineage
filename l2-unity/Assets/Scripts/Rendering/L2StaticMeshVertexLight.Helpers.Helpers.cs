using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2StaticMeshVertexLight
{
    static float SamplePoint(Vector3 lightPos, float radius, Vector3 worldPos, Vector3 normal)
    {
        Vector3 delta = lightPos - worldPos;
        float dist = delta.magnitude;
        if (radius <= 1e-4f || dist >= radius || dist <= 1e-4f)
            return 0f;
        float facing = Vector3.Dot(normal, delta);
        if (facing <= 0f)
            return 0f;
        float t = dist / radius;
        float ndot = facing / dist;
        float falloff = (1f - t) * (1f - t) * (1f + 2f * t);
        return 2f * ndot * falloff;
    }

    static float SampleSpot(Light light, Vector3 worldPos, Vector3 normal)
    {
        float baseIntensity = SamplePoint(light.Position, light.Radius, worldPos, normal);
        if (baseIntensity <= 0f)
            return 0f;
        Vector3 toVert = worldPos - light.Position;
        float dist = toVert.magnitude;
        if (dist <= 1e-4f || light.Direction.sqrMagnitude <= 1e-8f)
            return 0f;
        float cos = Vector3.Dot(light.Direction.normalized, toVert) / dist;
        float cone = 1f - light.Cone / 256f;
        float span = 1f - cone;
        if (span <= 1e-4f || cos <= cone)
            return 0f;
        float spot = (cos - cone) / span;
        return baseIntensity * spot * spot;
    }

    static float SampleDirectional(Vector3 direction, Vector3 normal)
    {
        if (direction.sqrMagnitude <= 1e-8f)
            return 0f;
        float dot = Vector3.Dot(normal, direction.normalized);
        if (dot >= 0f)
            return 0f;
        return -2f * dot;
    }
}
