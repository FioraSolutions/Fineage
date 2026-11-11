using UnityEngine;

public static partial class NpcDecoAttachment
{
    public static NpcDecoAttachPoint FromPieceName(string pieceName)
    {
        if (string.IsNullOrEmpty(pieceName))
            return NpcDecoAttachPoint.Bone;

        string n = pieceName.ToLowerInvariant();
        // Skill _ca is cast timing, not feet. ra_boss_halo_a_ca is still bone-attached.
        if (n.EndsWith("_feet") || n.Contains("_feet_") || n.Contains("_ground"))
            return NpcDecoAttachPoint.Feet;
        if (n.EndsWith("_oh") || n.Contains("_oh_") || n.Contains("_head") ||
            n.Contains("overhead") || n.Contains("_hat"))
            return NpcDecoAttachPoint.OverHead;
        return NpcDecoAttachPoint.Bone;
    }

    static Transform ResolveBone(Entity entity)
    {
        if (entity == null)
            return null;

        Gear gear = entity.Gear;
        if (gear == null)
            return null;

        for (int i = 0; i < BoneHints.Length; i++)
        {
            Transform bone = gear.FindRecursiveBone(BoneHints[i]);
            if (bone != null)
                return bone;
        }

        return null;
    }

    static bool TryResolver(
        Entity entity,
        EffectAttachmentPoint point,
        out Transform parent,
        out Vector3 worldPosition)
    {
        var context = new EffectResolveContext
        {
            CasterEntity = entity,
            TargetEntity = entity,
            CasterTransform = entity.transform,
            TargetTransform = entity.transform
        };

        if (Resolver.Resolve(point, context, out parent, out worldPosition))
        {
            if (parent == null)
                parent = entity.transform;
            return true;
        }

        parent = entity.transform;
        worldPosition = entity.transform.position;
        return parent != null;
    }
}
