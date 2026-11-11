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
            EffectAttachmentPoint.LeftWeaponSocket,
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
            EffectAttachmentPoint.WorldHitPoint,
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
            EffectAttachmentPoint.TargetCenter,
            context,
            out followTransform,
            out worldPosition);
    }
}
