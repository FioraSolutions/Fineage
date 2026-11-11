using UnityEngine;

public sealed partial class HomeFlightPart : CompositePart
{
    public override void ConfigurePlayback(
        BaseEffect instance,
        EffectSettings settings,
        MagicCastData castData,
        EffectResolveContext context)
    {
        // UC SpriteEmitter5: Lifetime=0.01 + 3000 pps + MaxParticles=3. Live
        // recycles those slots for the whole FNMover trip. Fixed-duration
        // burst would spawn once and vanish in a frame (SpriteEmitter2's
        // 0.33s trail still reads; the core does not).
        ParticleEmitterV2.BindHostOwnedEmission(instance);
    }

    public override string Describe()
    {
        return base.Describe() + " homeFlight launch=" + launchMode +
               " speed=" + speed.ToString("0.###") +
               " accel=" + acceleration.ToString("0.##") +
               " max=" + maxSpeed.ToString("0.##") +
               " side=" + pathSideOffset.ToString("0.##") +
               " height=" + pathHeightOffset.ToString("0.##") +
               " dual=" + (mirrorDualFlight ? "1" : "0");
    }

    public override void OnAfterSpawn(BaseEffect instance, EffectResolveContext context)
    {
        if (instance == null)
        {
            return;
        }

        instance.PrepareDestroyOnHomeArrival();
        EffectPart[] parts = instance.GetComponentsInChildren<EffectPart>(true);
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null)
            {
                parts[i].SetOwnerWorldPosOverride(true, instance.transform.position);
            }
        }

        if (ShouldHideUntilShoot())
        {
            CompositeProjectileLaunchHelper.SetPartVisualsVisible(instance.transform, false);
        }
    }
}
