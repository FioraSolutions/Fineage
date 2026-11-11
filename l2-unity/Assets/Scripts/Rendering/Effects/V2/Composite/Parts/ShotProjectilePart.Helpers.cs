using UnityEngine;
using System;

public sealed partial class ShotProjectilePart : CompositePart
{
    public override void OnAfterSpawn(BaseEffect instance, EffectResolveContext context)
    {
        if (ShouldHideUntilShoot())
        {
            CompositeProjectileLaunchHelper.SetPartVisualsVisible(instance != null ? instance.transform : null, false);
        }
    }

    public override void OnAnimationShoot(BaseEffect instance, EffectResolveContext context)
    {
        if (instance == null)
        {
            return;
        }

        if (ShouldHideUntilShoot())
        {
            L2Particle particle = instance as L2Particle;
            if (particle != null)
            {
                particle.ResetTimer();
            }

            CompositeProjectileLaunchHelper.SetPartVisualsVisible(instance.transform, true);
        }
    }

    bool ShouldHideUntilShoot()
    {
        if (spawnTiming == CompositePartSpawnTiming.OnHitCollider ||
            spawnTiming == CompositePartSpawnTiming.OnAnimationShoot)
        {
            return false;
        }

        return launchMode == ProjectileLaunchMode.OnAnimationShoot && !showBeforeAnimationShoot;
    }

    public bool TryLaunch(BaseEffect instance, EffectResolveContext context)
    {
        if (instance == null || context == null)
        {
            Debug.LogWarning("[ShotOrb] TryLaunch abort null instance/context part=" + name);
            return false;
        }

        Transform target = context.TargetTransform;
        if (target == null)
        {
            target = ResolveFallbackTarget(context);
            if (target != null)
            {
                context.TargetTransform = target;
            }
        }

        IProjectileManager projectiles = ProjectilePacketServices.Projectiles;
        if (target == null || projectiles == null)
        {
            Debug.LogWarning(
                "[ShotOrb] TryLaunch abort part=" + name +
                " target=" + (target != null) +
                " projectiles=" + (projectiles != null) +
                " castTargetId=" + (context.CastData != null ? context.CastData.TargetObjectId : 0));
            return false;
        }

        Transform projectileTransform = instance.transform;
        projectileTransform.SetParent(null, true);
        Vector3 startPos = projectileTransform.position;
        ProjectileData settings = new ProjectileData
        {
            prefab = instance.gameObject,
            transform = projectileTransform,
            startPosition = startPos,
            targetTransform = target,
            impactType = impactType,
            speed = speed > 0.01f ? speed : 10f
        };
        Debug.Log(
            "[ShotOrb] launch part=" + name +
            " go=" + instance.name +
            " start=" + startPos +
            " target=" + target.name);
        projectiles.LaunchProjectile(
            instance.gameObject,
            startPos,
            target,
            settings);
        return true;
    }

    static Transform ResolveFallbackTarget(EffectResolveContext context)
    {
        if (context == null)
        {
            return null;
        }

        if (context.TargetEntity != null)
        {
            return context.TargetEntity.transform;
        }

        int targetId = 0;
        if (context.CastData != null && context.CastData.TargetObjectId > 0)
        {
            targetId = context.CastData.TargetObjectId;
        }
        else if (context.CasterEntity != null && context.CasterEntity.TargetId > 0)
        {
            targetId = context.CasterEntity.TargetId;
        }

        World world = WorldApplyPacketServices.GameWorld;
        if (targetId <= 0 || world == null)
        {
            return null;
        }

        Entity targetEntity = world.GetEntityNoLockSync(targetId);
        if (targetEntity == null)
        {
            return null;
        }

        context.TargetEntity = targetEntity;
        context.TargetUserId = targetEntity.Identity != null
            ? targetEntity.Identity.Id
            : targetId;
        return targetEntity.transform;
    }
}
