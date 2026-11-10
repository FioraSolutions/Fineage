using System.Linq;
using UnityEngine;
using VContainer;

public interface IEffectAttachmentResolver
{
    bool Resolve(EffectAttachmentPoint point, EffectResolveContext context, out Transform resolvedTransform, out Vector3 worldPosition);
}

public partial class DefaultEffectAttachmentResolver : IEffectAttachmentResolver
{
    [Inject] World _world;

    private World GameWorld =>
        _world != null ? _world : GamePacketServices.GameWorld;

    private static readonly string[] WeaponNamePattern = { Gear.weaponName }; // "weapon_"

    public bool Resolve(EffectAttachmentPoint point, EffectResolveContext context, out Transform resolvedTransform, out Vector3 worldPosition)
    {
        resolvedTransform = null;
        worldPosition = Vector3.zero;

        if (!TryHydrateContext(context))
        {
            return false;
        }

        switch (point)
        {
            case EffectAttachmentPoint.CasterRoot:
                return ResolveRoot(context.CasterTransform, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.CasterLowerBody:
                return ResolveLowerBody(context.CasterEntity, context.CasterTransform, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.WeaponSocket:
                return ResolveWeapon(context.CasterEntity, context.CasterTransform, leftHand: false, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.LeftWeaponSocket:
                return ResolveWeapon(context.CasterEntity, context.CasterTransform, leftHand: true, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.TargetRoot:
                return ResolveRoot(context.TargetTransform, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.TargetLowerBody:
                return ResolveLowerBody(context.TargetEntity, context.TargetTransform, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.TargetCenter:
                return ResolveTargetCenter(context.TargetEntity, context.TargetTransform, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.TargetOverHead:
                return ResolveTargetOverHead(context.TargetEntity, context.TargetTransform, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.CasterCenter:
                return ResolveTargetCenter(context.CasterEntity, context.CasterTransform, out resolvedTransform, out worldPosition);
            case EffectAttachmentPoint.WorldHitPoint:
            {
                if (context.HasHitPoint)
                {
                    worldPosition = context.HitPoint;
                    return true;
                }

                Transform fallbackTarget = context.TargetTransform != null
                    ? context.TargetTransform
                    : context.CasterTransform;
                Entity fallbackEntity = context.TargetEntity != null
                    ? context.TargetEntity
                    : context.CasterEntity;
                Transform hitAnchor = HitAnchorResolver.ResolveHitAnchor(fallbackEntity, fallbackTarget);
                if (hitAnchor == null)
                {
                    return false;
                }

                worldPosition = hitAnchor.position;
                return true;
            }
            case EffectAttachmentPoint.CasterPosition:
                if (context.CasterTransform != null)
                {
                    worldPosition = context.CasterTransform.position;
                    return true;
                }
                return false;
            case EffectAttachmentPoint.TargetPosition:
                if (context.TargetTransform != null)
                {
                    worldPosition = context.TargetTransform.position;
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    private bool TryHydrateContext(EffectResolveContext context)
    {
        if (context == null)
        {
            return false;
        }

        World world = GameWorld;
        if (context.CasterEntity == null && context.CasterUserId > 0 && world != null)
        {
            context.CasterEntity = world.GetEntityNoLockSync(context.CasterUserId);
        }
        if (context.TargetEntity == null && context.TargetUserId > 0 && world != null)
        {
            context.TargetEntity = world.GetEntityNoLockSync(context.TargetUserId);
        }

        if (context.CasterTransform == null && context.CasterEntity != null)
        {
            context.CasterTransform = context.CasterEntity.transform;
        }
        if (context.TargetTransform == null && context.TargetEntity != null)
        {
            context.TargetTransform = context.TargetEntity.transform;
        }

        return context.CasterTransform != null || context.TargetTransform != null || context.HasHitPoint;
    }






}
