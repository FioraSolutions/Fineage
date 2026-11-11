using UnityEngine;


[System.Serializable]
public sealed class ShotProjectilePart : CompositePart
{
    public ProjectileLaunchMode launchMode = ProjectileLaunchMode.OnAnimationShoot;
    public bool showBeforeAnimationShoot;
    public ProjectileImpactType impactType = ProjectileImpactType.EffectOnly;
    public float speed;

    public override bool WantsAnimationShoot =>
        spawnTiming == CompositePartSpawnTiming.OnAnimationShoot ||
        launchMode == ProjectileLaunchMode.OnAnimationShoot;

    public override bool IsLaunchedProjectile =>
        launchMode == ProjectileLaunchMode.Immediate ||
        launchMode == ProjectileLaunchMode.OnAnimationShoot;

    public override bool UsesCastHitLifetime => false;

    public override void ConfigurePlayback(
        BaseEffect instance,
        EffectSettings settings,
        MagicCastData castData,
        EffectResolveContext context)
    {
        // Clock is ProjectileManager flytime (ProjectileFlightTimeCalculator at launch
        // distance), not castData.FlightTime / EffectCastDurationResolver (those only
        // shift the animation shoot: HitTime − FlightTime).
        float flytime = ResolveLaunchFlytimeSeconds(instance, context);
        if (settings != null)
        {
            settings.defaultLifeTime = flytime;
            settings.hideTime = 0f;
        }

        L2Particle particle = instance as L2Particle;
        if (particle != null)
        {
            particle.BindLifetimeToProjectile();
        }

        if (instance == null)
        {
            return;
        }

        ParticleEmitterV2.SetEmissionWindow(instance, flytime, EmitterStopMode.Drain, skipFixedDuration: false);
        if (instance != null && instance.name.IndexOf("el_wind_strike_fl", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            string caster = context != null && context.CasterEntity != null
                ? context.CasterEntity.name
                : "none";
            Debug.Log(
                "[WINDORB] configure caster=" + caster +
                " flytime=" + flytime.ToString("0.###") +
                " target=" + (context != null && context.TargetTransform != null
                    ? context.TargetTransform.name
                    : "none"));
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // "[ShotProjectilePart] '" + name + "' emit=ProjectileFlightTimeCalculator flytime=" +
            // flytime.ToString("0.###") + "s hideTime=0 drain-until-destroy emitters=" +
            // ParticleEmitterV2.InChildren(instance).Length);
#endif
    }

    static float ResolveLaunchFlytimeSeconds(BaseEffect instance, EffectResolveContext context)
    {
        if (instance == null || context == null || context.TargetTransform == null)
        {
            return 2f;
        }

        Vector3 start = instance.transform.position;
        Vector3 aim = VectorWorldUtils.GetCollision(start, context.TargetTransform);
        float distance = Vector3.Distance(start, aim);
        return ProjectileFlightTimeCalculator.CalculateL2AccelFlightTimeSeconds(distance);
    }

    public override string Describe()
    {
        return base.Describe() + " launch=" + launchMode;
    }

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

    public bool ShouldLaunchImmediately => launchMode == ProjectileLaunchMode.Immediate;

    public bool ShouldLaunchOnShoot => launchMode == ProjectileLaunchMode.OnAnimationShoot;

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
