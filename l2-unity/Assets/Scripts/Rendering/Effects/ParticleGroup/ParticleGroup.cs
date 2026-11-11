using Unity.Jobs;
using UnityEngine;

/// <summary>
/// Thin Unity orchestrator. Inspector fields stay here for prefab serialization.
/// Playback, Jobs, and renderer work live in dedicated models.
/// </summary>
public partial class ParticleGroup : EffectPart
{
    [SerializeField] private L2Particle _owner;
    [SerializeField] private Renderer[] _particles;
    [Header("Spawning (Настройки появления)")]
    [SerializeField] private float _startDelay = 0f;
    [SerializeField] private int _countPerSecond = 15;
    [SerializeField] private int _maxCount = 2;
    [SerializeField] private bool _cloneParticlesToMaxCount;
    [SerializeField] private int _cloneParticleLimit = 64;
    [SerializeField] private bool _forceContinuousSpawning;
    [Tooltip("Batch identical mesh+material slots into one DrawCall. Shader must tag L2FxGpuInstancing=On.")]
    [SerializeField] private bool _useGpuInstancing = true;
    [Tooltip("Continuous loop keeps already-active particles alive without resetting shader _StartTime/_Seed. Use for effects whose shader animation must keep running, for example mesh spin.")]
    [SerializeField] private bool _preserveShaderTimeInContinuousLoop;

    [Space(10)]
    [SerializeField] private bool _isBurstSpawning;
    [SerializeField] private float _relativeWarmupTime;

    [Header("Loop & Timing")]
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private bool _hasFixedDuration = true;
    [SerializeField] private bool _instantKillAtCastEnd;
    [SerializeField] private bool _fitToBounds;

    [Header("Home Projectile Flight")]
    [SerializeField] private bool _homeFlightAnchor;
    [SerializeField] private float _homePathSideOffsetMultiplier = 1f;
    [SerializeField] private float _homePathSideOffsetScale = 1f;
    [SerializeField] private float _homePathHeightOffsetScale = 1f;
    [SerializeField] private float _homeFlightSpeedScale = 1f;

    readonly ParticleGroupPlaybackState _playback = new ParticleGroupPlaybackState();
    readonly ParticleGroupSimulation _simulation = new ParticleGroupSimulation();
    ParticleGroupRendererService _renderers;
    ParticleGroupHomeFlightProfile _runtimeHomeFlightProfile;
    bool _hasRuntimeHomeFlightProfile;
    float[] _particleSpawnTimes;
    bool[] _isParticleActive;

    public bool IsHomeFlightAnchor =>
        _homeFlightAnchor || (_hasRuntimeHomeFlightProfile && _runtimeHomeFlightProfile.isFlightAnchor);

}
