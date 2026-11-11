using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
    void RunL2Warmup(float now)
    {
        float relative = Mathf.Max(0f, _authoring.relativeWarmupTime);
        if (relative <= 0f)
        {
            return;
        }

        float ticksPerSec = _authoring.warmupTicksPerSecond > 0.01f
            ? _authoring.warmupTicksPerSecond
            : 10f;
        float lifeCenter = ReadLifetimeCenter(_authoring.duration > 0f ? _authoring.duration : _particleLifetime);
        float warmupTime = lifeCenter * relative;
        float warmupDelta = 1f / ticksPerSec;
        int ticks = (int)(ticksPerSec * warmupTime);
        if (ticks <= 0)
        {
            return;
        }

        for (int i = 0; i < ticks; i++)
        {
            _warmupAgeAtSpawn = (ticks - 1 - i) * warmupDelta;
            _simulation.ScheduleExpire(now, _particleLifetime);
            _simulation.CompleteExpire();
            _slots.ApplyExpire(_simulation, _active, _spawnTimes);
            Spawn(now, warmupDelta);
        }

        _warmupAgeAtSpawn = 0f;
        if (WindOrbTrace.Matches(_host))
        {
            Debug.Log(
                "[WINDORB] warmup emitter=" + DebugName +
                " caster=" + WindOrbTrace.Caster(_host) +
                " lifeCenter=" + lifeCenter.ToString("0.###") +
                " particleLife=" + _particleLifetime.ToString("0.###") +
                " relative=" + relative.ToString("0.###") +
                " warmupTime=" + warmupTime.ToString("0.###") +
                " ticks=" + ticks +
                " spawned=" + _spawn.SpawnedTotal +
                " active=" + CountActive());
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // "[ParticleGroupV2] L2 warmup '" + (string.IsNullOrEmpty(DebugName) ? "?" : DebugName) +
            // "' lifeCenter=" + lifeCenter.ToString("0.###") +
            // " relative=" + relative.ToString("0.###") +
            // " warmupTime=" + warmupTime.ToString("0.###") +
            // " tps=" + ticksPerSec.ToString("0.###") +
            // " ticks=" + ticks +
            // " dt=" + warmupDelta.ToString("0.###") +
            // " spawned=" + _spawn.SpawnedTotal);
#endif
    }
}
