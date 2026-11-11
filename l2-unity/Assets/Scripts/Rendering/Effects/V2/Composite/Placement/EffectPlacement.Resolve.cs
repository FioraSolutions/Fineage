using UnityEngine;

public partial class EffectPlacement
{
    public static EffectPlacement FromAttachment(EffectAttachmentPoint point, string boneName)
    {
        if (!string.IsNullOrWhiteSpace(boneName))
        {
            return new BonePlacement { boneName = boneName };
        }

        switch (point)
        {
            case EffectAttachmentPoint.CasterRoot:
                return new FeetPlacement();
            case EffectAttachmentPoint.WeaponSocket:
                return new WeaponPlacement();
            case EffectAttachmentPoint.LeftWeaponSocket:
                return new LeftWeaponPlacement();
            case EffectAttachmentPoint.WorldHitPoint:
                return new HitPointPlacement();
            case EffectAttachmentPoint.TargetCenter:
            case EffectAttachmentPoint.TargetRoot:
            case EffectAttachmentPoint.TargetLowerBody:
            case EffectAttachmentPoint.TargetPosition:
                return new TargetCenterPlacement();
            case EffectAttachmentPoint.TargetOverHead:
                return new TargetOverHeadPlacement();
            case EffectAttachmentPoint.CasterPosition:
                return new CasterPositionPlacement();
            default:
                return new ChestPlacement();
        }
    }

    public static bool MatchesAttachment(EffectPlacement placement, EffectAttachmentPoint point)
    {
        return placement != null &&
               !(placement is BonePlacement) &&
               ToAttachment(placement) == point;
    }

    public static EffectAttachmentPoint ToAttachment(EffectPlacement placement)
    {
        if (placement is TargetOverHeadPlacement)
        {
            return EffectAttachmentPoint.TargetOverHead;
        }

        if (placement is TargetCenterPlacement)
        {
            return EffectAttachmentPoint.TargetCenter;
        }

        if (placement is FeetPlacement)
        {
            return EffectAttachmentPoint.CasterRoot;
        }

        if (placement is WeaponPlacement)
        {
            return EffectAttachmentPoint.WeaponSocket;
        }

        if (placement is LeftWeaponPlacement)
        {
            return EffectAttachmentPoint.LeftWeaponSocket;
        }

        if (placement is HitPointPlacement)
        {
            return EffectAttachmentPoint.WorldHitPoint;
        }

        if (placement is CasterPositionPlacement)
        {
            return EffectAttachmentPoint.CasterPosition;
        }

        if (placement is ChestPlacement)
        {
            return EffectAttachmentPoint.CasterCenter;
        }

        return EffectAttachmentPoint.CasterRoot;
    }

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
