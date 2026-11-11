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

    void Spawn(float now, float deltaTime)
    {
        int maxCount = Mathf.Max(1, _authoring.maxCount);
        int remaining = _authoring.respawnDeadParticles
            ? CountInactiveSlots()
            : Mathf.Max(0, maxCount - _spawn.SpawnedTotal);
        if (remaining <= 0)
        {
            if (!_authoring.respawnDeadParticles)
            {
                Stop(EmitterStopMode.Drain);
            }

            return;
        }

        if (_authoring.isBurstSpawning && !_spawn.BurstFinished)
        {
            int burst = remaining;
            for (int i = 0; i < burst; i++)
            {
                if (!TryActivateFreeSlot(now))
                {
                    break;
                }
            }

            _spawn.BurstFinished = true;
            if (!_authoring.respawnDeadParticles)
            {
                Stop(EmitterStopMode.Drain);
            }

            return;
        }

        int toSpawn = _spawn.ConsumeRate(deltaTime, _authoring.countPerSecond, remaining);
        for (int i = 0; i < toSpawn; i++)
        {
            if (!TryActivateFreeSlot(now))
            {
                return;
            }
        }

        if (!_authoring.respawnDeadParticles && _spawn.SpawnedTotal >= maxCount)
        {
            Stop(EmitterStopMode.Drain);
        }
    }

    bool TryActivateFreeSlot(float now)
    {
        int slot = FindFreeSlot();
        if (slot < 0)
        {
            return false;
        }

        float shaderStartTime = now - _warmupAgeAtSpawn;
        float seed = UnityEngine.Random.Range(-100f, 100f);
        Vector4 spawnLocationAddUe = ResolveSpawnLocationAddUe(now);
        CaptureSpawnObjectToWorld(slot);
        _simulation.EnsureLifetime(_slots.Count);
        if (_batch.Bound)
        {
            _simulation.ActivateGpuSlot(
                slot,
                now,
                shaderStartTime,
                seed,
                _meshRandBase,
                _spriteRandBase,
                _active,
                _spawnTimes,
                spawnLocationAddUe);
        }
        else
        {
            Renderer renderer = _slots.Particles != null && slot < _slots.Particles.Length
                ? _slots.Particles[slot]
                : null;
            if (renderer != null)
            {
                _slots.ApplyCoordinateSystemToGoSlot(slot, _authoring.coordinateSystem);
                renderer.gameObject.SetActive(true);
            }

            _simulation.MarkSlotActive(slot, shaderStartTime, _active, _spawnTimes);
            _slots.ActivateGoSlot(slot, shaderStartTime, seed, _meshRandBase, _spriteRandBase);
            ApplyLifeStretchToRenderer(renderer);
        }

        _spawn.SpawnedTotal++;
        _cursor = slot + 1;
        if (_cursor >= _slots.Count)
        {
            _cursor = 0;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!_loggedFirstSpawn)
        {
            _loggedFirstSpawn = true;
            float shaderAge = now - shaderStartTime;
            bool deadInShader = shaderAge >= _particleLifetime - 0.01f;
            // Debug.Log(
                // "[ParticleGroupV2] first spawn '" + (string.IsNullOrEmpty(DebugName) ? "?" : DebugName) +
                // "' slot=" + slot +
                // "/" + _slots.Count +
                // " gpu=" + _batch.Bound +
                // " now=" + now.ToString("0.###") +
                // " shaderStart=" + shaderStartTime.ToString("0.###") +
                // " shaderAge=" + shaderAge.ToString("0.###") +
                // " simLife=" + _particleLifetime.ToString("0.###") +
                // (deadInShader ? " DEAD_IN_SHADER_AT_SPAWN" : string.Empty));
            ParticleGroupV2 compareHost = _host as ParticleGroupV2;
            if (compareHost != null && compareHost.CompareLogEnabled)
            {
                // ParticleGroupV2CompareLog.WriteSpawn(
                    // compareHost,
                    // slot,
                    // _slots.Count,
                    // now,
                    // shaderStartTime,
                    // _particleLifetime);
            }

            // if (NpcDeco2911Trace.Matches(DebugName))
            {
                // NpcDeco2911Trace.Log(
                    // "FIRST SPAWN slot=" + slot +
                    // " gpu=" + _batch.Bound +
                    // " spawnedTotal=" + _spawn.SpawnedTotal +
                    // (deadInShader ? " DEAD_IN_SHADER_AT_SPAWN" : " alive"));
            }
        }
#endif

        return true;
    }
}
