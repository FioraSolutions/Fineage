using UnityEngine;


[System.Serializable]
public sealed class StationaryPart : CompositePart
{
    public override void ConfigurePlayback(
        BaseEffect instance,
        EffectSettings settings,
        MagicCastData castData,
        EffectResolveContext context)
    {
        if (instance == null)
        {
            return;
        }

        float window;
        if (settings != null && settings.useConfigLifetime)
        {
            window = ParticleEmitterV2.MaxAuthoredDuration(instance, 0.01f);
        }
        else
        {
            window = EffectCastDurationResolver.Resolve(
                0.01f,
                false,
                settings,
                castData,
                out _,
                out _);
        }

        ParticleEmitterV2.SetEmissionWindow(instance, window, EmitterStopMode.Drain, skipFixedDuration: true);
    }
}
