using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

/// <summary>Shared distance + frustum sphere test for pawns and instanced map deco.</summary>
public static class VisibilitySphereFrustumCull
{
    public const int FrustumPlaneCount = 6;

    public static void FillFrustumPlanes(Camera camera, Plane[] scratch, NativeArray<float4> nativePlanes)
    {
        GeometryUtility.CalculateFrustumPlanes(camera, scratch);
        for (int p = 0; p < FrustumPlaneCount; p++)
        {
            Plane plane = scratch[p];
            Vector3 n = plane.normal;
            nativePlanes[p] = new float4(n.x, n.y, n.z, plane.distance);
        }
    }

    public static bool IsVisible(
        float3 center,
        float radius,
        float3 cameraPosition,
        float maxDrawDistance,
        NativeArray<float4> frustumPlanes)
    {
        float limit = maxDrawDistance + radius;
        if (math.distancesq(center, cameraPosition) > limit * limit)
            return false;

        for (int p = 0; p < frustumPlanes.Length; p++)
        {
            float4 plane = frustumPlanes[p];
            float dist = math.dot(plane.xyz, center) + plane.w;
            if (dist < -radius)
                return false;
        }

        return true;
    }
}