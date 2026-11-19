using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2OverheadAnchor
{
    public static void Invalidate(int entityId)
    {
        if (entityId != 0)
            _headBoneByEntity.Remove(entityId);
    }

    public static void InvalidateTransform(Transform root)
    {
        if (root != null)
            _headBoneByInstance.Remove(root.GetInstanceID());
    }

    public static void ClearAll()
    {
        _headBoneByEntity.Clear();
        _headBoneByInstance.Clear();
    }

    public static Vector3 GetWorldPos(Entity entity, Transform root)
    {
        if (root == null)
            return Vector3.zero;

        int id = entity != null && entity.Identity != null ? entity.Identity.Id : 0;
        return ResolveWorldPos(id, useEntityCache: true, root, CollisionHeightOf(entity));
    }
}
