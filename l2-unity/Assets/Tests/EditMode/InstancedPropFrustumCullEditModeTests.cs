using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public sealed class InstancedPropFrustumCullEditModeTests
{
  [Test]
  public void InstancedPropFrustumCullJob_rejects_sphere_behind_camera_plane()
  {
    var centers = new NativeArray<float3>(1, Allocator.Temp);
    var radii = new NativeArray<float>(1, Allocator.Temp);
    var planes = new NativeArray<float4>(1, Allocator.Temp);
    var visible = new NativeArray<byte>(1, Allocator.Temp);

  // Camera looks +Z: near plane at origin with normal -Z pushes culled objects behind camera.
    centers[0] = new float3(0f, 0f, -10f);
    radii[0] = 1f;
    planes[0] = new float4(0f, 0f, -1f, 0f);

    var job = new InstancedPropFrustumCullJob
    {
        WorldCenters = centers,
        Radii = radii,
        FrustumPlanes = planes,
        CameraPosition = float3.zero,
        MaxDrawDistance = 100f,
        Visible = visible,
    };
    job.Execute(0);

    Assert.AreEqual(0, visible[0]);

    centers.Dispose();
    radii.Dispose();
    planes.Dispose();
    visible.Dispose();
  }

  [Test]
  public void InstancedPropFrustumCullJob_accepts_sphere_inside_view()
  {
    var centers = new NativeArray<float3>(1, Allocator.Temp);
    var radii = new NativeArray<float>(1, Allocator.Temp);
    var planes = new NativeArray<float4>(6, Allocator.Temp);
    var visible = new NativeArray<byte>(1, Allocator.Temp);

    centers[0] = new float3(0f, 0f, 10f);
    radii[0] = 2f;
    Camera cam = new GameObject("CullTestCam").AddComponent<Camera>();
    Plane[] frustum = new Plane[6];
    GeometryUtility.CalculateFrustumPlanes(cam, frustum);
    for (int i = 0; i < 6; i++)
    {
      Vector3 n = frustum[i].normal;
      planes[i] = new float4(n.x, n.y, n.z, frustum[i].distance);
    }

    var job = new InstancedPropFrustumCullJob
    {
        WorldCenters = centers,
        Radii = radii,
        FrustumPlanes = planes,
        CameraPosition = cam.transform.position,
        MaxDrawDistance = 50f,
        Visible = visible,
    };
    job.Execute(0);

    Assert.AreEqual(1, visible[0]);

    Object.DestroyImmediate(cam.gameObject);
    centers.Dispose();
    radii.Dispose();
    planes.Dispose();
    visible.Dispose();
  }
}