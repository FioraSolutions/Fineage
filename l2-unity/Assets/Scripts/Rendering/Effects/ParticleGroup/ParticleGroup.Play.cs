using Unity.Jobs;
using UnityEngine;

public partial class ParticleGroup
{
    public override void PlayPart()
    {
        if (!isActiveAndEnabled)
            return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // if (!_playback.Stopped)
            // EffectDoublePlayLog.Repeat(
                // "ParticleGroup.PlayPart",
                // this,
                // _playback.LastEnable,
                // _playback.FirstPlayStack);
#endif
        ParticleGroupJobPump.Remove(this);
        _simulation.CompleteExpire();

        if (_fitToBounds && OwnerTarget != null)
            FitToOwnerWidth(OwnerTarget);

        Renderers.SetParticles(_particles);
        Renderers.CollectIfEmpty();
        float shaderSlotDuration = Renderers.ReadShaderSlotDuration(_duration);
        if (_duration < 0.01f || _duration < shaderSlotDuration)
            _duration = shaderSlotDuration;

        Renderers.EnsureClones(_cloneParticlesToMaxCount, _maxCount, _cloneParticleLimit, ref _playback.RuntimeClonesCreated);
        _particles = Renderers.Particles;
        Renderers.ExpandShaderDrivenBounds();

        EnsureManagedSlots(Renderers.Count);
        _simulation.EnsureLifetime(Renderers.Count, reset: true);
        bool gpuBound = _simulation.TryBindGpu(_particles, _useGpuInstancing);
        if (gpuBound)
            Renderers.DisableForGpuDraw();
        else
            Renderers.EnableForGameObjectDraw();

        _playback.BaseShaderLifetime = Mathf.Max(0.01f, Renderers.ReadLifetimeMax(_duration));
        _playback.Begin(Now(), _countPerSecond, _hasFixedDuration, _duration);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // _playback.FirstPlayStack = EffectDoublePlayLog.CaptureStack();
#endif
        Renderers.HideAll();
    }

    public override void Setup(EffectSettings s, MagicCastData c)
    {
        _settings = s;
        _castData = c;
        if (!_hasFixedDuration)
        {
            _duration = EffectCastDurationResolver.Resolve(
                _duration,
                _hasFixedDuration,
                _settings,
                _castData,
                out _,
                out _);
        }
    }

    public override void StopPart()
    {
        _playback.StopSpawning();
        if (_instantKillAtCastEnd)
        {
            DeactivateAllParticles();
            _playback.Stopped = true;
        }
    }

    public ParticleGroupAuthoring CaptureAuthoring()
    {
        return new ParticleGroupAuthoring
        {
            particles = _particles,
            startDelay = _startDelay,
            countPerSecond = _countPerSecond,
            maxCount = _maxCount,
            cloneToMaxCount = _cloneParticlesToMaxCount,
            cloneLimit = _cloneParticleLimit,
            useGpuInstancing = _useGpuInstancing,
            isBurstSpawning = _isBurstSpawning,
            relativeWarmupTime = _relativeWarmupTime,
            duration = _duration,
            hasFixedDuration = _hasFixedDuration,
            instantKillAtCastEnd = _instantKillAtCastEnd,
            // Cast-window parts keep emitting into free slots until StopPart.
            // Active slots are never overwritten.
            respawnDeadParticles = !_hasFixedDuration
        };
    }

    public void SetRuntimeContinuousLoopOverride(bool hasOverride, bool value) =>
        _playback.SetLoopOverride(hasOverride, value);

    public void FitToOwnerWidth(Transform target)
    {
        if (target == null)
            return;
        var controller = target.GetComponent<CharacterController>();
        if (controller == null)
            return;
        float targetWidth = controller.radius * 2f;
        transform.localScale = new Vector3(targetWidth * 4f, 1f, targetWidth * 4f);
    }
}
