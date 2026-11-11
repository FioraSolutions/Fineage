using Unity.Jobs;
using UnityEngine;

public partial class ParticleGroup
{
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
