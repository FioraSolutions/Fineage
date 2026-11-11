using System.Linq;
using UnityEngine;
using VContainer;

public interface IEffectAttachmentResolver
{
    bool Resolve(EffectAttachmentPoint point, EffectResolveContext context, out Transform resolvedTransform, out Vector3 worldPosition);
}

public partial class DefaultEffectAttachmentResolver : IEffectAttachmentResolver
{
    [Inject] IWorldEntityLookup _entities;
    [Inject] World _world;

    private World GameWorld =>
        _world != null ? _world : GamePacketServices.GameWorld;

    private IWorldEntityLookup Entities =>
        _entities != null ? _entities : GameWorld;

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
                // Never fall back to the caster — that paints impact FX on the mage.
                Transform targetRoot = context.TargetTransform;
                Entity targetEntity = context.TargetEntity;
                if (targetRoot == null && targetEntity == null)
                {
                    return false;
                }

                if (context.HasHitPoint)
                {
                    worldPosition = context.HitPoint;
                    resolvedTransform = targetRoot != null
                        ? targetRoot
                        : HitAnchorResolver.ResolveHitAnchor(targetEntity, null);
                    return true;
                }

                Transform hitAnchor = HitAnchorResolver.ResolveHitAnchor(targetEntity, targetRoot);
                if (hitAnchor == null)
                {
                    return false;
                }

                resolvedTransform = hitAnchor;
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

        // Entity stays ACS-typed until Entity moves (#131); Transform via IWorldEntityLookup.
        World world = GameWorld;
        if (context.CasterEntity == null && context.CasterUserId > 0 && world != null)
        {
            context.CasterEntity = world.GetEntityNoLockSync(context.CasterUserId);
        }
        if (context.TargetEntity == null && context.TargetUserId > 0 && world != null)
        {
            context.TargetEntity = world.GetEntityNoLockSync(context.TargetUserId);
        }

        IWorldEntityLookup entities = Entities;
        if (context.CasterTransform == null)
        {
            if (context.CasterEntity != null)
                context.CasterTransform = context.CasterEntity.transform;
            else if (context.CasterUserId > 0 && entities != null)
                context.CasterTransform = entities.FindTransformNoLock(context.CasterUserId);
        }
        if (context.TargetTransform == null)
        {
            if (context.TargetEntity != null)
                context.TargetTransform = context.TargetEntity.transform;
            else if (context.TargetUserId > 0 && entities != null)
                context.TargetTransform = entities.FindTransformNoLock(context.TargetUserId);
        }

        return context.CasterTransform != null || context.TargetTransform != null || context.HasHitPoint;
    }
}
