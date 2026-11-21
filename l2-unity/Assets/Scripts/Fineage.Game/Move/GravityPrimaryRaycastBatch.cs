using Unity.Collections;
using UnityEngine;

/// <summary>
/// Batched primary downward probes for gravity snap (first pass of WorldPlacement.CollectWalkableYs).
/// </summary>
public static class GravityPrimaryRaycastBatch
{
  // Matches WorldPlacement.DefaultStartAbove + DefaultMaxDistance (50 + 300).
  public const float StartAbove = 50f;
  public const float MaxDistance = 350f;

  public static void Probe(
      NativeArray<Vector3> references,
      LayerMask mask,
      NativeArray<RaycastHit> hits)
  {
    int count = references.Length;
    if (count == 0)
      return;

    var commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);
    for (int i = 0; i < count; i++)
    {
      Vector3 p = references[i];
      float originY = p.y + StartAbove;
      var origin = new Vector3(p.x, originY, p.z);
      commands[i] = new RaycastCommand(origin, Vector3.down, MaxDistance, mask, maxHits: 1);
    }

    var handle = RaycastCommand.ScheduleBatch(commands, hits, 1, default);
    handle.Complete();
    commands.Dispose();
  }
}