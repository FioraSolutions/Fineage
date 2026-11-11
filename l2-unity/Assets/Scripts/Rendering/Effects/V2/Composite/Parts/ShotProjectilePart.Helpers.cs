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
        if (instance == null || context == null || context.TargetTransform == null || ProjectilePacketServices.Projectiles == null)
        {
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
            targetTransform = context.TargetTransform,
            impactType = impactType,
            speed = speed > 0.01f ? speed : 10f
        };
        if (instance.name.IndexOf("el_wind_strike_fl", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            string caster = context.CasterEntity != null ? context.CasterEntity.name : "none";
            Debug.Log(
                "[WINDORB] launch caster=" + caster +
                " start=" + startPos +
                " target=" + context.TargetTransform.name);
        }
        ProjectilePacketServices.Projectiles.LaunchProjectile(
            instance.gameObject,
            startPos,
            context.TargetTransform,
            settings);
        return true;
    }
}
