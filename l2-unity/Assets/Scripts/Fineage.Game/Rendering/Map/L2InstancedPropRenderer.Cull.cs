using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public partial class L2InstancedPropRenderer
{
  NativeArray<float3> _worldCenters;
  NativeArray<float> _cellRadii;
  NativeArray<byte> _visible;
  int _cellCullCapacity;
  Plane[] _frustumScratch = new Plane[6];
  NativeArray<float4> _frustumPlanesNative;
  bool _hasCullBuffers;

  void EnsureCullBuffers(int count)
  {
    if (_cellCullCapacity >= count && _hasCullBuffers)
      return;
    DisposeCullBuffers();
    _cellCullCapacity = math.max(16, count);
    var persistent = Allocator.Persistent;
    _worldCenters = new NativeArray<float3>(_cellCullCapacity, persistent);
    _cellRadii = new NativeArray<float>(_cellCullCapacity, persistent);
    _visible = new NativeArray<byte>(_cellCullCapacity, persistent);
    _frustumPlanesNative = new NativeArray<float4>(6, persistent);
    _hasCullBuffers = true;
  }

  void DisposeCullBuffers()
  {
    if (!_hasCullBuffers)
      return;
    if (_worldCenters.IsCreated) _worldCenters.Dispose();
    if (_cellRadii.IsCreated) _cellRadii.Dispose();
    if (_visible.IsCreated) _visible.Dispose();
    if (_frustumPlanesNative.IsCreated) _frustumPlanesNative.Dispose();
    _cellCullCapacity = 0;
    _hasCullBuffers = false;
  }

  void RefreshCellRadii()
  {
    if (_cells == null)
      return;
    int count = _cells.Count;
    EnsureCullBuffers(count);
    for (int i = 0; i < count; i++)
      _cellRadii[i] = _cells[i].radius;
  }

  void CullAndDraw(Camera camera, Matrix4x4 world, float maxDistance, Vector3 cameraPos)
  {
    int count = _cells.Count;
    if (count == 0)
      return;

    EnsureCullBuffers(count);

    GeometryUtility.CalculateFrustumPlanes(camera, _frustumScratch);
    for (int p = 0; p < 6; p++)
    {
      Plane plane = _frustumScratch[p];
      Vector3 n = plane.normal;
      _frustumPlanesNative[p] = new float4(n.x, n.y, n.z, plane.distance);
    }

    for (int i = 0; i < count; i++)
    {
      Vector3 localCenter = _cells[i].center;
      Vector3 worldCenter = world.MultiplyPoint3x4(localCenter);
      _worldCenters[i] = new float3(worldCenter.x, worldCenter.y, worldCenter.z);
    }

    using (FineagePerfProfiling.DecoInstancedCull.Auto())
    {
      var job = new InstancedPropFrustumCullJob
      {
          WorldCenters = _worldCenters,
          Radii = _cellRadii,
          FrustumPlanes = _frustumPlanesNative,
          CameraPosition = new float3(cameraPos.x, cameraPos.y, cameraPos.z),
          MaxDrawDistance = maxDistance,
          Visible = _visible,
      };
      job.Schedule(count, 32).Complete();
    }

    for (int i = 0; i < count; i++)
    {
      if (_visible[i] == 0)
        continue;
      DrawCell(_cells[i], world);
    }
  }

  void OnDestroy()
  {
    DisposeCullBuffers();
  }
}