using UnityEngine;

public sealed partial class ParticleStreamDriver : EffectPart, IParticleEmitterV2
{
    ParticleStreamRuntime _runtime;
    ParticleGroupAuthoring _authoring;
    float _emissionWindow = 1f;
    ParticleLifetimePolicy _lifetimePolicy = ParticleLifetimePolicy.Authored;
    bool _bound;
    bool _streamVisible = true;
    bool _destroying;
    bool _hasExternalEmissionWindow;

    public bool IsGpuDraw => _runtime != null && _runtime.IsGpuDraw;
    public ParticleLifetimePolicy LifetimePolicy => _lifetimePolicy;
    public bool HasFixedDuration => _authoring.hasFixedDuration;
    public bool IsComplete => _runtime != null && _runtime.IsComplete;
    public float AuthoredDuration => Mathf.Max(0.01f, _authoring.duration);












}
