using UnityEngine;

public sealed partial class ShotProjectilePart : CompositePart
{
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
}
