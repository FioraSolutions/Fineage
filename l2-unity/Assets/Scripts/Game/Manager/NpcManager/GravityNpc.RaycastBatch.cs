using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public partial class GravityNpc
{
  readonly List<int> _gravityBatchKeys = new List<int>(64);
  readonly List<Vector3> _gravityReferences = new List<Vector3>(64);
  readonly List<Transform> _gravityBodies = new List<Transform>(64);
  readonly List<Entity> _gravityEntities = new List<Entity>(64);
  readonly List<Vector3> _gravityFeet = new List<Vector3>(64);

  void StepGravityWithBatchRays()
  {
    _gravityBatchKeys.Clear();
    _gravityReferences.Clear();
    _gravityBodies.Clear();
    _gravityEntities.Clear();
    _gravityFeet.Clear();

    foreach (KeyValuePair<int, GravityData> kvp in _dict)
    {
      GravityData data = kvp.Value;
      CharacterController controller = data.GetControllerToTypeEntity();
      if (controller == null || !controller.enabled || !controller.gameObject.activeInHierarchy)
      {
        _remove.Add(kvp.Key);
        continue;
      }

      if (data.IsDead())
        continue;

      if (data.ArmFrames > 0)
      {
        data.ArmFrames--;
        continue;
      }

      Transform body = controller.transform;
      Vector3 feet = body.position;
      Entity entity = data.Entity;
      float refY = feet.y;
      if (entity != null && entity.Identity != null)
      {
        entity.Identity.RememberServerPosition();
        refY = entity.Identity.ServerPosition.y;
      }

      _gravityBatchKeys.Add(kvp.Key);
      _gravityFeet.Add(feet);
      _gravityReferences.Add(new Vector3(feet.x, refY, feet.z));
      _gravityBodies.Add(body);
      _gravityEntities.Add(entity);
    }

    int count = _gravityReferences.Count;
    if (count == 0)
      return;

    LayerMask mask = WorldPlacement.CurrentWalkableMask;
    var nativeRefs = new NativeArray<Vector3>(count, Allocator.TempJob);
    var nativeHits = new NativeArray<RaycastHit>(count, Allocator.TempJob);
    for (int i = 0; i < count; i++)
      nativeRefs[i] = _gravityReferences[i];

    using (FineagePerfProfiling.GravityRaycastBatch.Auto())
      GravityPrimaryRaycastBatch.Probe(nativeRefs, mask, nativeHits);

    for (int i = 0; i < count; i++)
    {
      Vector3 feet = _gravityFeet[i];
      Transform body = _gravityBodies[i];
      Entity entity = _gravityEntities[i];
      Vector3 reference = _gravityReferences[i];

      Vector3 stood;
      if (!WorldPlacement.TrySnapWalkableFromRaycastHit(reference, nativeHits[i], out stood))
        stood = WorldPlacement.KeepOnBrush(reference);

      if ((stood - feet).sqrMagnitude < 0.0001f)
        continue;

      WorldPlacement.ApplyPose(body.gameObject, stood, body.rotation);
      if (entity != null && entity.Identity != null)
        entity.Identity.Position = stood;
    }

    nativeRefs.Dispose();
    nativeHits.Dispose();
  }
}