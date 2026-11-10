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
        float seed = Random.Range(-100f, 100f);
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

    int FindFreeSlot()
    {
        int count = _slots.Count;
        if (count <= 0 || _active == null)
        {
            return -1;
        }

        for (int i = 0; i < count; i++)
        {
            int slot = (_cursor + i) % count;
            if (!IsSlotActive(slot))
            {
                return slot;
            }
        }

        return -1;
    }

    bool HasAnyActiveSlot()
    {
        int count = _slots.Count;
        if (count <= 0 || _active == null)
        {
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            if (IsSlotActive(i))
            {
                return true;
            }
        }

        return false;
    }

    int CountInactiveSlots()
    {
        int count = _slots.Count;
        if (count <= 0)
        {
            return 0;
        }

        int inactive = 0;
        for (int i = 0; i < count; i++)
        {
            if (!IsSlotActive(i))
            {
                inactive++;
            }
        }

        return inactive;
    }

    bool IsSlotActive(int slot)
    {
        bool simActive = _simulation.HasLifetimeBuffers && _simulation.IsActive(slot);
        return simActive || (_active != null && slot < _active.Length && _active[slot]);
    }

    void KillAll()
    {
        _simulation.ClearAllActive(_active);
        _slots.HideAll();
    }

    void EnsureSlots(int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (_active != null && _active.Length == count)
        {
            for (int i = 0; i < count; i++)
            {
                _active[i] = false;
                _spawnTimes[i] = 0f;
            }

            return;
        }

        _active = new bool[count];
        _spawnTimes = new float[count];
    }

    void EnsureCoordinateMatrices(int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (_spawnObjectToWorld == null || _spawnObjectToWorld.Length != count)
        {
            _spawnObjectToWorld = new Matrix4x4[count];
            _hasSpawnObjectToWorld = new bool[count];
            return;
        }

        for (int i = 0; i < _hasSpawnObjectToWorld.Length; i++)
        {
            _hasSpawnObjectToWorld[i] = false;
        }
    }

    void CaptureSpawnObjectToWorld(int slot)
    {
        if (!L2ParticleCoordinateSystemUtil.FreezesSpawnMatrix(_authoring.coordinateSystem))
        {
            return;
        }

        Matrix4x4[] current = _slots.ResolveObjectToWorldMatrices();
        if (current == null || slot < 0 || slot >= current.Length)
        {
            return;
        }

        Matrix4x4 matrix = current[slot];
        if (_authoring.coordinateSystem == L2ParticleCoordinateSystem.Independent)
        {
            matrix = Matrix4x4.TRS(
                matrix.GetColumn(3),
                Quaternion.identity,
                new Vector3(
                    matrix.GetColumn(0).magnitude,
                    matrix.GetColumn(1).magnitude,
                    matrix.GetColumn(2).magnitude));
        }

        _spawnObjectToWorld[slot] = matrix;
        _hasSpawnObjectToWorld[slot] = true;
    }

    Matrix4x4[] ResolveDrawMatrices(Matrix4x4[] current)
    {
        if (!L2ParticleCoordinateSystemUtil.FreezesSpawnMatrix(_authoring.coordinateSystem) ||
            current == null ||
            _spawnObjectToWorld == null)
        {
            return current;
        }

        int count = Mathf.Min(current.Length, _spawnObjectToWorld.Length);
        for (int i = 0; i < count; i++)
        {
            if (_hasSpawnObjectToWorld[i])
            {
                current[i] = _spawnObjectToWorld[i];
            }
        }

        return current;
    }

    void ApplyLifeStretchToRenderer(Renderer renderer)
    {
        if (!ShouldStretchLife())
        {
            return;
        }

        L2FxAdjustParticleLife.ApplyToRenderer(
            renderer,
            _authoring.authoredParticleLife,
            _authoring.targetParticleLife);
    }

    void ApplyLifeStretchToGpuMaterials()
    {
        if (!ShouldStretchLife() || !_batch.Bound)
        {
            return;
        }

        Material[] materials = _simulation.GpuMaterials;
        if (materials == null)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            L2FxAdjustParticleLife.ApplyToMaterial(
                materials[i],
                _authoring.authoredParticleLife,
                _authoring.targetParticleLife);
        }
    }

    bool ShouldStretchLife()
    {
        return _authoring.stretchParticleLifeToWindow &&
               Mathf.Abs(_authoring.targetParticleLife - _authoring.authoredParticleLife) > 1e-4f;
    }

    Vector4 ResolveSpawnLocationAddUe(float spawnTime)
    {
        if (_host == null)
        {
            return Vector4.zero;
        }

        IParticleSpawnLocationAddProvider provider =
            _host.GetComponentInParent<IParticleSpawnLocationAddProvider>();
        if (provider != null &&
            provider.TryGetSpawnLocationAddUe(_host, spawnTime, out Vector4 addUe))
        {
            return addUe;
        }

        return Vector4.zero;
    }
}
