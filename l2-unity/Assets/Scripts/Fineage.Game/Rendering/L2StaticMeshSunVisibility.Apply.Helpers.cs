using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2StaticMeshSunVisibility
{
    static bool IsOccluded(
        PhysicsScene physicsScene,
        Vector3 origin,
        Vector3 direction,
        float distance,
        int collisionMask,
        ICollection<Collider> ownerColliders,
        RaycastHit[] hits)
    {
        int count = physicsScene.Raycast(
            origin,
            direction,
            hits,
            distance,
            collisionMask,
            QueryTriggerInteraction.Ignore);

        if (count <= 0)
            return false;

        Array.Sort(hits, 0, count, RaycastHitDistanceComparer.Instance);
        int inspected = Mathf.Min(count, MaxTraceHits);
        for (int i = 0; i < inspected; i++)
        {
            Collider collider = hits[i].collider;
            if (collider == null)
                continue;
            if (ownerColliders != null && ownerColliders.Contains(collider))
                continue;
            return true;
        }

        return false;
    }
}
