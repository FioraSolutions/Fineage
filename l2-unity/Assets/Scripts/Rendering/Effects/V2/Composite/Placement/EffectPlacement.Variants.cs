using UnityEngine;

[System.Serializable]
public sealed class FeetPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.CasterRoot,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class ChestPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.CasterCenter,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class WeaponPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.WeaponSocket,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class LeftWeaponPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.LeftWeaponSocket,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class HitPointPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.WorldHitPoint,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class TargetCenterPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.TargetCenter,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class TargetOverHeadPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.TargetOverHead,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class CasterPositionPlacement : EffectPlacement
{
    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        return resolver.Resolve(
            EffectAttachmentPoint.CasterPosition,
            context,
            out followTransform,
            out worldPosition);
    }
}

[System.Serializable]
public sealed class BonePlacement : EffectPlacement
{
    public string boneName;

    public override bool TryResolve(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        if (!string.IsNullOrEmpty(boneName) &&
            context != null &&
            context.CasterEntity != null &&
            context.CasterEntity.Gear != null)
        {
            Transform bone = context.CasterEntity.Gear.FindRecursiveBone(boneName);
            if (bone != null)
            {
                followTransform = bone;
                worldPosition = bone.position;
                return true;
            }
        }

        return resolver.Resolve(
            EffectAttachmentPoint.CasterRoot,
            context,
            out followTransform,
            out worldPosition);
    }
}
