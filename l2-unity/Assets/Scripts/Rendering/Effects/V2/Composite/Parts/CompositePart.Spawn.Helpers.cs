using UnityEngine;

public sealed partial class HomeFlightPart : CompositePart
{
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

    public bool TryLaunch(BaseEffect instance, EffectResolveContext context, IHomeProjectileService homeProjectiles)
    {
        if (instance == null || context == null || context.CasterTransform == null || homeProjectiles == null)
        {
            return false;
        }

        homeProjectiles.EnsureDualFlightRoots(instance, mirrorDualFlight);
        return homeProjectiles.Launch(instance, context, BuildConfig());
    }

    CompositeHomeProjectileConfig BuildConfig()
    {
        return new CompositeHomeProjectileConfig
        {
            launchMode = launchMode,
            speed = speed,
            acceleration = acceleration,
            maxSpeed = maxSpeed,
            fadeStartDistance = fadeStartDistance,
            fadeOutSeconds = fadeOutSeconds,
            arriveDistance = arriveDistance,
            maxLifetime = maxLifetime,
            homeAttachmentPoint = homeAttachmentPoint,
            homeOffset = homeOffset,
            usePathArc = usePathArc,
            pathStartLineFactor = pathStartLineFactor,
            pathPeelAlongLine = pathPeelAlongLine,
            pathApexAlongLine = pathApexAlongLine,
            pathPeakHeightAlongLine = pathPeakHeightAlongLine,
            pathSideOffset = pathSideOffset,
            pathHeightOffset = pathHeightOffset,
            pathDistanceHeightFactor = pathDistanceHeightFactor,
            pathEarlyClimbFactor = pathEarlyClimbFactor,
            pathAscentSpeedScale = pathAscentSpeedScale,
            pathDescentSpeedScale = pathDescentSpeedScale,
            rotateToVelocity = rotateToVelocity,
            destroyOnArrive = destroyOnArrive,
            mirrorDualFlight = mirrorDualFlight
        };
    }

    bool ShouldHideUntilShoot()
    {
        if (spawnTiming == CompositePartSpawnTiming.OnHitCollider ||
            spawnTiming == CompositePartSpawnTiming.OnAnimationShoot)
        {
            return false;
        }

        return launchMode == ProjectileLaunchMode.OnAnimationShoot;
    }
}
