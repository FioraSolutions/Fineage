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
    bool visible = VisibilitySphereFrustumCull.IsVisible(
        WorldCenters[index],
        Radii[index],
        CameraPosition,
        MaxDrawDistance,
        FrustumPlanes);
    Visible[index] = (byte)(visible ? 1 : 0);
  }
}