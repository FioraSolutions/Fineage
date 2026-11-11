using Unity.Jobs;
using UnityEngine;

public partial class ParticleGroup
{
    public void FixedUpdate()
    {
        if (_playback.Stopped || Renderers.Count == 0)
            return;

        float now = Now();
        if (!_playback.SpawnStopped && now - _playback.LastEnable < _startDelay)
            return;

        // _duration is the authored/server slot window. Using shader lifetime
        // here makes continuous caster effects recycle early and visibly replay.
        _simulation.ScheduleExpire(now, _duration);
        ParticleGroupJobPump.Enqueue(this, now);
    }

    internal void ApplyExpireAndSpawn(float now)
    {
        if (_playback.Stopped)
            return;

        bool anyActive = Renderers.ApplyExpire(_simulation, _isParticleActive, _particleSpawnTimes);
        if (_playback.SpawnStopped)
        {
            if (!anyActive)
                _playback.Stopped = true;
            return;
        }

        bool loop = _playback.ShouldLoopContinuously(_forceContinuousSpawning);
        if (_playback.SpawnedCount < _maxCount || loop)
        {
            if (_isBurstSpawning && !loop && !_playback.BurstFinished)
            {
                ActivateBurst(now);
                _playback.BurstFinished = true;
            }
            else if (now - _playback.LastLoop >= 1f / _countPerSecond)
            {
                _playback.LastLoop = now;
                ActivateParticle(now);
                _playback.SpawnedCount++;
            }
        }
        else if (!anyActive)
        {
            _playback.Stopped = true;
        }
    }

    void LateUpdate()
    {
        if (_playback.Stopped || !_simulation.CanPackAndDraw)
            return;

        _simulation.CompleteExpire();
        _simulation.PackAndDraw(
            Renderers.ResolveGpuOwnerWorldPos(_simulation.GpuMaterials),
            Renderers.ResolveObjectToWorldMatrices());
    }

    void OnDisable()
    {
        ParticleGroupJobPump.Remove(this);
        _simulation.CompleteExpire();
        if (_isParticleActive != null)
            Renderers.ApplyExpire(_simulation, _isParticleActive, _particleSpawnTimes);
    }

    void OnDestroy()
    {
        ParticleGroupJobPump.Remove(this);
        _simulation.Dispose();
    }
}
