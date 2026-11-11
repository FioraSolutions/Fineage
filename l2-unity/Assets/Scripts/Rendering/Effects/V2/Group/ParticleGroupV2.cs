using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thin V2 emitter: authored fields + stream + slot set + draw batch.
/// ParticleGroup vs ParticleSingle is flags (maxCount, respawn, stretch), not a second class.
/// No loop/hold, JobPump, or home-flight. Legacy ParticleGroup stays on old prefabs.
/// </summary>
public sealed partial class ParticleGroupV2 : EffectPart, IParticleEmitterV2
{
    [SerializeField] L2Particle _owner;
    [SerializeField] Renderer[] _particles;
    [Header("Spawning (UC)")]
    [SerializeField] float _startDelay;
    [SerializeField] int _countPerSecond = 15;
    [SerializeField] int _maxCount = 1;
    [HideInInspector] [SerializeField] bool _cloneParticlesToMaxCount;
    [HideInInspector] [SerializeField] int _cloneParticleLimit = 64;
    [HideInInspector] [SerializeField] bool _useGpuInstancing = true;
    [HideInInspector] [SerializeField] L2ParticleCoordinateSystem _coordinateSystem;
    [SerializeField] bool _isBurstSpawning;
    [HideInInspector] [SerializeField] float _relativeWarmupTime;
    [HideInInspector] [SerializeField] float _warmupTicksPerSecond;
    [Header("Timing (UC)")]
    [SerializeField] float _duration = 0.2f;
    [SerializeField] bool _hasFixedDuration = true;
    [HideInInspector] [SerializeField] bool _instantKillAtCastEnd;
    [Tooltip("UC RespawnDeadParticles. Independent of HasFixedDuration.")]
    [SerializeField] bool _respawnDeadParticles = true;
    [HideInInspector]
    [Tooltip("UC MaxParticles=1 + no respawn: AdjustparticleLife stretches shader life to HitTime.")]
    [SerializeField] bool _stretchParticleLifeToWindow;
    [Tooltip("NSkillProjectile / NPC deco: keep emitting until the host GO is destroyed.")]
    [SerializeField] bool _hostOwnedEmission;
    [Tooltip("Write HE-style EffectLog snapshot to Logs/ParticleGroupV2Compare.txt")]
    [SerializeField] bool _compareLogEnabled;

    ParticleSlotSet _slots;
    ParticleDrawBatch _batch;
    ParticleStreamRuntime _runtime;
    ParticleLifetimePolicy _lifetimePolicy = ParticleLifetimePolicy.Authored;
    float _emissionWindow = 1f;
    bool _hasExternalEmissionWindow;
    bool _streamVisible = true;
    bool _destroying;
    bool _forceAuthoredTiming;
    Material[] _runtimeMaterialInstances;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    bool _loggedHiddenTick;
#endif
    bool _windFlightTrace;
    string _windCaster;
    float _windPlayAt;
    int _windTraceStage;

    public bool CompareLogEnabled => _compareLogEnabled;
    public bool IsGpuDraw => _runtime != null && _runtime.IsGpuDraw;
    public Material[] GpuMaterials => _runtime != null ? _runtime.GpuMaterials : null;
    public bool IsComplete => _runtime != null && _runtime.IsComplete;
    public ParticleLifetimePolicy LifetimePolicy => _lifetimePolicy;
    public bool HasFixedDuration => _hasFixedDuration;
    public float AuthoredDuration => Mathf.Max(0.01f, _duration);
    public ParticleDrawBatch DrawBatch
    {
        get
        {
            EnsureRuntime();
            return _batch;
        }
    }


#if UNITY_EDITOR || DEVELOPMENT_BUILD
    string GetHierarchyPath()
    {
        Transform current = transform;
        string path = current.name;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }

        return path;
    }
#endif
}

static class WindOrbTrace
{
    public static bool Matches(Component component)
    {
        Transform current = component != null ? component.transform : null;
        while (current != null)
        {
            if (current.name.IndexOf("el_wind_strike_fl", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    public static string Caster(Component component)
    {
        Transform current = component != null ? component.transform : null;
        while (current != null)
        {
            Entity entity = current.GetComponent<Entity>();
            if (entity != null)
            {
                return entity.name;
            }

            current = current.parent;
        }

        return "detached";
    }
}
