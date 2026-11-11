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

    public bool InstantKillAtCastEnd => _authoring.instantKillAtCastEnd;
    public bool IsComplete => _state == EmitterState.Complete;
    public bool IsGpuDraw => _batch.Bound;
    public int SlotCount => _slots != null ? _slots.Count : 0;
    public int SpawnedTotal => _spawn.SpawnedTotal;
    public Material[] GpuMaterials => _simulation != null ? _simulation.GpuMaterials : null;
}
