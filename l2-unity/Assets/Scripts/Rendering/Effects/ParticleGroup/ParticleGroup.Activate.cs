using Unity.Jobs;
using UnityEngine;

public partial class ParticleGroup
{
    void ActivateBurst(float now)
    {
        bool skipRestart = _playback.ShouldLoopContinuously(_forceContinuousSpawning) &&
                           _preserveShaderTimeInContinuousLoop;
        if (_simulation.TryActivateGpuBurst(
            now,
            _relativeWarmupTime,
            _maxCount,
            skipRestart,
            _playback.MeshRandBase,
            _playback.SpriteRandBase,
            ref _playback.ParticleIndex,
            _isParticleActive,
            _particleSpawnTimes))
        {
            _playback.SpawnedCount += _maxCount;
            return;
        }

        for (int i = 0; i < _maxCount; i++)
        {
            ActivateParticle(now);
            _playback.SpawnedCount++;
        }
    }

    void ActivateParticle(float now)
    {
        if (Renderers.Count == 0)
            return;

        _playback.WrapIndex(Renderers.Count);
        EnsureManagedSlots(Renderers.Count);
        _simulation.EnsureLifetime(Renderers.Count);

        if (_simulation.GpuEnabled)
        {
            ActivateGpuParticle(now);
            return;
        }

        Renderer renderer = Renderers.Particles[_playback.ParticleIndex];
        GameObject particleObject = renderer.gameObject;
        bool loop = _playback.ShouldLoopContinuously(_forceContinuousSpawning);
        if (loop &&
            _isParticleActive[_playback.ParticleIndex] &&
            particleObject.activeSelf &&
            _preserveShaderTimeInContinuousLoop)
        {
            Renderers.UpdateWorldPositions(renderer);
            _playback.AdvanceIndex(Renderers.Count);
            return;
        }

        particleObject.SetActive(true);
        int slot = _playback.ParticleIndex;
        float shaderStartTime = now - _relativeWarmupTime;
        _simulation.MarkSlotActive(
            slot,
            now,
            _isParticleActive,
            _particleSpawnTimes);
        Renderers.ActivateGoSlot(
            slot,
            shaderStartTime,
            Random.Range(-100f, 100f),
            _playback.MeshRandBase,
            _playback.SpriteRandBase);
        _playback.AdvanceIndex(Renderers.Count);
    }

    void ActivateGpuParticle(float now)
    {
        bool loop = _playback.ShouldLoopContinuously(_forceContinuousSpawning);
        if (loop &&
            _isParticleActive[_playback.ParticleIndex] &&
            _preserveShaderTimeInContinuousLoop)
        {
            _playback.AdvanceIndex(Renderers.Count);
            return;
        }

        int slot = _playback.ParticleIndex;
        float shaderStartTime = now - _relativeWarmupTime;
        _simulation.ActivateGpuSlot(
            slot,
            now,
            shaderStartTime,
            Random.Range(-100f, 100f),
            _playback.MeshRandBase,
            _playback.SpriteRandBase,
            _isParticleActive,
            _particleSpawnTimes);
        _playback.AdvanceIndex(Renderers.Count);
    }

    void DeactivateAllParticles()
    {
        ParticleGroupJobPump.Remove(this);
        _simulation.CompleteExpire();
        if (Renderers.Count == 0)
            return;

        EnsureManagedSlots(Renderers.Count);
        _simulation.EnsureLifetime(Renderers.Count);
        _simulation.ClearAllActive(_isParticleActive);
        Renderers.HideAll();
    }

    void EnsureManagedSlots(int count)
    {
        if (count <= 0)
            return;
        if (_particleSpawnTimes != null && _particleSpawnTimes.Length == count)
            return;

        _particleSpawnTimes = new float[count];
        _isParticleActive = new bool[count];
    }
}
