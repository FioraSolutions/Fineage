using Unity.Jobs;
using UnityEngine;

public partial class ParticleGroup
{
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
