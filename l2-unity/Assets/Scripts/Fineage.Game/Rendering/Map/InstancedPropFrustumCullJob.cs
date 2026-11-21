using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

/// <summary>
/// Distance + frustum sphere cull for <see cref="L2InstancedPropRenderer"/> cells (roadmap step 18).
/// </summary>
[BurstCompile]
public struct InstancedPropFrustumCullJob : IJobParallelFor
{
  [ReadOnly] public NativeArray<float3> WorldCenters;
  [ReadOnly] public NativeArray<float> Radii;
  [ReadOnly] public NativeArray<float4> FrustumPlanes;
  public float3 CameraPosition;
  public float MaxDrawDistance;
  public NativeArray<byte> Visible;

  public void Execute(int index)
  {
    float3 center = WorldCenters[index];
    float radius = Radii[index];

    float limit = MaxDrawDistance + radius;
    if (math.distancesq(center, CameraPosition) > limit * limit)
    {
      Visible[index] = 0;
      return;
    }

    for (int p = 0; p < FrustumPlanes.Length; p++)
    {
      float4 plane = FrustumPlanes[p];
      float3 normal = plane.xyz;
      float dist = math.dot(normal, center) + plane.w;
      if (dist < -radius)
      {
        Visible[index] = 0;
        return;
      }
    }

    Visible[index] = 1;
  }
}