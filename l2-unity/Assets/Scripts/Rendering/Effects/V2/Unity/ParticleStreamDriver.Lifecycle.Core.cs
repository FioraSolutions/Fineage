using UnityEngine;

public sealed partial class ParticleStreamDriver : EffectPart, IParticleEmitterV2
{
    public void Bind(ParticleGroupAuthoring authoring)
    {
        _authoring = authoring;
        _runtime ??= new ParticleStreamRuntime(this);
        _runtime.Bind(authoring);
        _runtime.SetVisible(_streamVisible);
        _bound = true;
    }

    public void SetStreamVisible(bool visible)
    {
        _streamVisible = visible;
        _runtime?.SetVisible(visible);
    }

    public override void Setup(EffectSettings settings, MagicCastData castData)
    {
        _settings = settings;
        _castData = castData;
        RecaptureAuthoring();
        ResolveEmissionWindow(settings, castData);
    }
}
