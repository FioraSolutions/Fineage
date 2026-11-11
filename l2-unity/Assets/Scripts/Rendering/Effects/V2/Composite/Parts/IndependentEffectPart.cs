using UnityEngine;


[System.Serializable]
public class IndependentEffectPart : CompositePart
{
    public override bool OutlivesComposite => true;

    public override bool UsesCastHitLifetime => false;

    public override void ConfigurePlayback(
        BaseEffect instance,
        EffectSettings settings,
        MagicCastData castData,
        EffectResolveContext context)
    {
        if (!follow)
        {
            DetachFromContainer(instance);
        }

        L2Particle particle = instance as L2Particle;
        if (particle != null && ParticleEmitterV2.InChildren(particle).Length > 0)
        {
            particle.BindLifetimeToAuthoredStreams();
            return;
        }

        float life = ParticleEmitterV2.MaxAuthoredDuration(instance, 0.01f);
        if (settings != null)
        {
            settings.defaultLifeTime = life;
            settings.hideTime = 0f;
        }
    }

    public override void OnAfterSpawn(BaseEffect instance, EffectResolveContext context)
    {
        if (!follow)
        {
            DetachFromContainer(instance);
        }
    }

    static void DetachFromContainer(BaseEffect instance)
    {
        if (instance == null)
        {
            return;
        }

        instance.transform.SetParent(null, true);
    }
}
