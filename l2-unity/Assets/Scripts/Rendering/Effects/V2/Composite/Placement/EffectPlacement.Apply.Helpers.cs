using UnityEngine;

public partial class EffectPlacement
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
