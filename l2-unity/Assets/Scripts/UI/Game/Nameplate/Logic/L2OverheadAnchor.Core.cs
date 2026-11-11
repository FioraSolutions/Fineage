using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2OverheadAnchor
{
    public static void Invalidate(int entityId)
    {
        if (entityId != 0)
            _localHeadYByEntity.Remove(entityId);
    }

    public static void InvalidateTransform(Transform root)
    {
        if (root != null)
            _localHeadYByInstance.Remove(root.GetInstanceID());
    }

    public static void ClearAll()
    {
        _localHeadYByEntity.Clear();
        _localHeadYByInstance.Clear();
    }

    public static Vector3 GetWorldPos(Entity entity, Transform root)
    {
        if (root == null)
            return Vector3.zero;

        int id = entity != null && entity.Identity != null ? entity.Identity.Id : 0;
        float localY = ResolveLocalHeadY(id, useEntityCache: true, root);
        return root.TransformPoint(new Vector3(0f, localY, 0f)) + Vector3.up * NameOffsetMeters;
    }
}
