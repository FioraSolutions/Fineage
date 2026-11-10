using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
    readonly EffectPart _host;
    readonly ParticleGroupSimulation _simulation = new ParticleGroupSimulation();
    readonly ParticleSlotSet _slots;
    readonly ParticleDrawBatch _batch;

    ParticleGroupAuthoring _authoring;
    bool[] _active;
    float[] _spawnTimes;
    Matrix4x4[] _spawnObjectToWorld;
    bool[] _hasSpawnObjectToWorld;
    int _cursor;
    uint _meshRandBase;
    uint _spriteRandBase;
    float _startedAt;
    float _emissionStartedAt;
    float _emissionEndsAt;
    float _particleLifetime;
    EmitterState _state = EmitterState.Idle;
    SpawnScheduler _spawn;
    bool _clonesCreated;
    float _lastTick = -1f;
    bool _visible = true;
    bool _needsWarmup;
    float _warmupAgeAtSpawn;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    bool _loggedFirstSpawn;
#endif

    public string DebugName;

    public ParticleStreamRuntime(EffectPart host)
        : this(host, new ParticleSlotSet(host), new ParticleDrawBatch())
    {
    }

    public ParticleStreamRuntime(EffectPart host, ParticleSlotSet slots, ParticleDrawBatch batch)
    {
        _host = host;
        _slots = slots ?? new ParticleSlotSet(host);
        _batch = batch ?? new ParticleDrawBatch();
    }

    public EmitterState State => _state;
    public bool InstantKillAtCastEnd => _authoring.instantKillAtCastEnd;
    public bool IsComplete => _state == EmitterState.Complete;
    public bool IsGpuDraw => _batch.Bound;
    public int SlotCount => _slots != null ? _slots.Count : 0;
    public int SpawnedTotal => _spawn.SpawnedTotal;

    public int CountActive()
    {
        if (_active == null)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < _active.Length; i++)
        {
            if (_active[i])
            {
                count++;
            }
        }

        return count;
    }
    public Material[] GpuMaterials => _simulation != null ? _simulation.GpuMaterials : null;

    public void Bind(ParticleGroupAuthoring authoring)
    {
        _authoring = authoring;
        _slots.SetParticles(authoring.particles);
        _slots.CollectIfEmpty();
    }

    public void SetVisible(bool visible)
    {
        _visible = visible;
        if (visible && _batch.Bound)
        {
            _slots.DisableForGpuDraw();
        }
    }

    public void Start(float now, float emissionWindow, float particleLifetime)
    {
        _simulation.CompleteExpire();
        _slots.EnsureClones(
            _authoring.cloneToMaxCount,
            _authoring.maxCount,
            Mathf.Max(8, _authoring.cloneLimit),
            ref _clonesCreated);
        _slots.ExpandShaderDrivenBounds();

        int count = _slots.Count;
        EnsureSlots(count);
        EnsureCoordinateMatrices(count);
        _simulation.EnsureLifetime(count, reset: true);
        bool gpu = _batch.TryBind(_slots.Particles, _authoring.useGpuInstancing, _simulation);
        if (gpu)
        {
            _slots.DisableForGpuDraw();
        }
        else
        {
            _slots.EnableForGameObjectDraw();
        }

        _slots.HideAll();
        float delay = Mathf.Max(0f, _authoring.startDelay);
        _startedAt = now;
        _emissionStartedAt = now + delay;
        if (_authoring.hostOwnedEmission)
        {
            _emissionEndsAt = float.PositiveInfinity;
        }
        else
        {
            _emissionEndsAt = now + Mathf.Max(0.01f, emissionWindow);
            if (_emissionEndsAt < _emissionStartedAt)
            {
                _emissionEndsAt = _emissionStartedAt;
            }
        }

        _particleLifetime = Mathf.Max(0.01f, particleLifetime);
        _cursor = 0;
        _meshRandBase = L2MaterialPropertyCopier.CreateFiniteAppRandState();
        _spriteRandBase = L2MaterialPropertyCopier.CreateFiniteAppRandState();
        _spawn.Reset();
        if (_authoring.countPerSecond > 0 && !_authoring.isBurstSpawning)
        {
            _spawn.Accumulator = 1f;
        }

        _lastTick = now;
        _state = EmitterState.Emitting;
        _needsWarmup = _authoring.relativeWarmupTime > 0f;
        _warmupAgeAtSpawn = 0f;
        ApplyLifeStretchToGpuMaterials();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        _loggedFirstSpawn = false;
        // if (NpcDeco2911Trace.Matches(DebugName) || NpcDeco2911Trace.Matches(_host != null ? _host.name : null))
        {
            // NpcDeco2911Trace.Log(
                // "Start gpu=" + gpu +
                // " slots=" + count +
                // " cps=" + _authoring.countPerSecond +
                // " max=" + _authoring.maxCount +
                // " hostOwned=" + _authoring.hostOwnedEmission +
                // " respawn=" + _authoring.respawnDeadParticles +
                // " life=" + _particleLifetime.ToString("0.###") +
                // " emitEnd=" +
                // (float.IsInfinity(_emissionEndsAt) ? "inf" : _emissionEndsAt.ToString("0.###")) +
                // (gpu
                    // ? " MeshRenderers DISABLED (GPU instancing draws them)"
                    // : " GO draw path"));
            // if (count <= 0)
                // NpcDeco2911Trace.Warn("Start SKIP — zero slots, nothing to spawn");
            // if (!gpu && _authoring.useGpuInstancing)
                // NpcDeco2911Trace.Warn("GPU bind FAILED, falling back to GameObject draw");
        }
#endif
    }

    public void Tick(float now)
    {
        if (!_visible || _state == EmitterState.Idle || _state == EmitterState.Complete)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // if (NpcDeco2911Trace.Matches(DebugName) && !_loggedFirstSpawn)
            {
                // NpcDeco2911Trace.Warn(
                    // "Tick SKIP visible=" + _visible +
                    // " state=" + _state +
                    // " — no spawn/draw this frame");
            }
#endif
            return;
        }

        if (_state == EmitterState.Emitting && now < _emissionStartedAt)
        {
            _lastTick = now;
            return;
        }

        if (_needsWarmup)
        {
            _needsWarmup = false;
            RunL2Warmup(now);
        }

        _simulation.ScheduleExpire(now, _particleLifetime);
        _simulation.CompleteExpire();
        _slots.ApplyExpire(_simulation, _active, _spawnTimes);

        if (_state == EmitterState.Emitting)
        {
            if (now >= _emissionEndsAt)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (_spawn.SpawnedTotal == 0)
                {
                    // Debug.LogWarning(
                        // "[ParticleGroupV2] emission ended with ZERO spawns '" +
                        // (string.IsNullOrEmpty(DebugName) ? "?" : DebugName) +
                        // "' now=" + now.ToString("0.###") +
                        // " emitStart=" + _emissionStartedAt.ToString("0.###") +
                        // " emitEnd=" + _emissionEndsAt.ToString("0.###") +
                        // " slots=" + SlotCount +
                        // " delayLeft=" + Mathf.Max(0f, _emissionStartedAt - now).ToString("0.###"));
                }
#endif
                Stop(_authoring.instantKillAtCastEnd ? EmitterStopMode.Kill : EmitterStopMode.Drain);
            }
            else
            {
                float dt = _lastTick > 0f ? Mathf.Max(0f, now - _lastTick) : Time.fixedDeltaTime;
                Spawn(now, dt);
            }
        }

        _lastTick = now;

        if (_state == EmitterState.Draining && !HasAnyActiveSlot())
        {
            _state = EmitterState.Complete;
        }
    }

    public void LateDraw()
    {
        if (!_visible ||
            _state == EmitterState.Idle ||
            _state == EmitterState.Complete ||
            !_batch.Bound)
        {
            return;
        }

        _simulation.CompleteExpire();
        Matrix4x4[] objectToWorldMatrices = _slots.ResolveObjectToWorldMatrices();
        _batch.Draw(
            _simulation,
            _slots.ResolveGpuOwnerWorldPos(_simulation.GpuMaterials),
            ResolveDrawMatrices(objectToWorldMatrices));
    }

    public void Stop(EmitterStopMode mode)
    {
        if (_state == EmitterState.Idle || _state == EmitterState.Complete)
        {
            return;
        }

        _state = EmitterState.Draining;
        if (mode == EmitterStopMode.Kill)
        {
            KillAll();
            _state = EmitterState.Complete;
        }
    }

    public float ReadParticleLifetime(float fallback)
    {
        return Mathf.Max(0.01f, _slots.ReadShaderSlotDuration(fallback));
    }

    public void Dispose()
    {
        _simulation.CompleteExpire();
        _simulation.Dispose();
        _slots.RestoreCoordinateSystemSlots();
        _batch.Release();
    }

    public float ReadLifetimeCenter(float fallback)
    {
        return Mathf.Max(0.01f, _slots.ReadLifetimeCenter(fallback));
    }

}
