using UnityEngine;

public sealed partial class ParticleStreamDriver : EffectPart, IParticleEmitterV2
{
    void FixedUpdate()
    {
        if (!_streamVisible)
        {
            return;
        }

        _runtime?.Tick(Now());
    }

    void LateUpdate()
    {
        if (!_streamVisible)
        {
            return;
        }

        _runtime?.LateDraw();
    }

    void OnDisable()
    {
        if (_runtime == null)
        {
            return;
        }

        _runtime.Stop(_destroying ? EmitterStopMode.Kill : EmitterStopMode.Drain);
    }

    void OnDestroy()
    {
        _destroying = true;
        _runtime?.Stop(EmitterStopMode.Kill);
        _runtime?.Dispose();
        _runtime = null;
    }

    void RecaptureAuthoring()
    {
        ParticleGroup group = GetComponent<ParticleGroup>();
        if (group != null)
        {
            Bind(group.CaptureAuthoring());
            return;
        }

        ParticleSingle single = GetComponent<ParticleSingle>();
        if (single != null)
        {
            Bind(single.CaptureAuthoring());
        }
    }

    void ResolveEmissionWindow(EffectSettings settings, MagicCastData castData)
    {
        if (_hasExternalEmissionWindow)
        {
            _lifetimePolicy = ParticleLifetimePolicy.EmissionWindowFromCast;
            _emissionWindow = Mathf.Max(0.01f, _emissionWindow);
            return;
        }

        if (_authoring.hasFixedDuration)
        {
            _lifetimePolicy = ParticleLifetimePolicy.Authored;
            _emissionWindow = Mathf.Max(0.01f, _authoring.duration);
            return;
        }

        _lifetimePolicy = ParticleLifetimePolicy.EmissionWindowFromCast;
        float fallback = Mathf.Max(0.01f, _authoring.duration);
        _emissionWindow = EffectCastDurationResolver.Resolve(
            fallback,
            false,
            settings,
            castData,
            out _,
            out _);
        if (_emissionWindow < 0.01f)
        {
            _emissionWindow = fallback;
        }
    }
}
