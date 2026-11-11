using UnityEngine;

public static partial class NpcDecoAttachment
{
    public static Transform Resolve(Entity entity)
    {
        return ResolveBone(entity) ?? (entity != null ? entity.transform : null);
    }

    public static Quaternion UprightYaw(Transform source)
        => NpcDecoUpright.UprightYaw(source);

    public static bool TryResolve(
        Entity entity,
        NpcDecoAttachPoint point,
        out Transform parent,
        out Vector3 worldPosition)
    {
        parent = null;
        worldPosition = Vector3.zero;
        if (entity == null)
            return false;

        switch (point)
        {
            case NpcDecoAttachPoint.Feet:
                return TryResolver(
                    entity,
                    EffectAttachmentPoint.CasterRoot,
                    out parent,
                    out worldPosition);
            case NpcDecoAttachPoint.OverHead:
                return TryResolver(
                    entity,
                    EffectAttachmentPoint.TargetOverHead,
                    out parent,
                    out worldPosition);
            default:
                parent = ResolveBone(entity) ?? entity.transform;
                if (parent == null)
                    return false;
                worldPosition = parent.position;
                return true;
        }
    }
}
