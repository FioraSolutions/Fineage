using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle : EffectPart
{
    private static readonly int AlphaShaderId = Shader.PropertyToID("_Alpha");
    private static readonly int SurfaceNormalsShaderId = Shader.PropertyToID("_SurfaceNormals");

    [SerializeField] private L2Particle _owner;
    [SerializeField] private Renderer[] _particles;

    [Header("Spawning (single slot)")]
    [SerializeField] private float _startDelay = 0f;
    [SerializeField] private int _countPerSecond = 15;
    [SerializeField] private int _maxCount = 1;
    [SerializeField] private bool _forceContinuousSpawning;
    [Tooltip("Do not reset _StartTime/_Seed while continuous loop is active.")]
    [SerializeField] private bool _preserveShaderTimeInContinuousLoop;

    [Space(10)]
    [SerializeField] private bool _isBurstSpawning;
    [SerializeField] private float _relativeWarmupTime;

    [Header("Loop & Timing")]
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private bool _hasFixedDuration = true;
    [SerializeField] private bool _instantKillAtCastEnd;
    [SerializeField] private bool _fitToBounds;

    [Tooltip("Если включено: один Spawn при PlayPart, без FixedUpdate-тайминга и без скрытия/остановки. Для проверки _Hold и прочего в материале вручную.")]
    [SerializeField] private bool _testSpawnOnlyNoTeardown = false;

    [Header("L2 shader (Hold / fade)")]
    [Tooltip("Значение _Hold в материале, если включён Continuous Loop части композита (overrideContinuousLoop + continuousLoop через SetRuntimeContinuousLoopOverride). Это не то же самое, что спавн-луп — см. код композита.")]
    [SerializeField] [Range(0f, 1f)] private float _shaderHold = 0.6f;
    [Tooltip("Иначе _Hold висит всё время каста и в L2Fx unified emitters может блокировать нормальный FadeOut.")]
    [SerializeField] private bool _releaseShaderHoldByCastProgress = true;
    [Tooltip("Доля времени каста (_duration): раньше этой точки _Hold = Shader Hold. При cast-end defer (телепорт) во время каста hold не отпускается — поле влияет только если defer выключен.")]
    [SerializeField] [Range(0f, 0.999f)] private float _shaderHoldReleaseStartNormalized = 0.85f;
    [Tooltip("Вкл.: от Release Start до конца каста _Hold линейно к 0. При defer до StopPart не используется.")]
    [SerializeField] private bool _smoothShaderHoldRelease = true;
    [Tooltip("Только cast-end (после StopPart): сжатие SizeScale стартует на эту долю hideTime раньше alpha fade. 0 = синхронно с fade, 0.1 = на 10% hideTime раньше.")]
    [SerializeField] [Range(0f, 0.5f)] private float _castEndHoldReleaseLead = 0.1f;

    private readonly ParticleSingleLifetimeTracker _lifetime = new ParticleSingleLifetimeTracker();
    private readonly L2ShaderHoldController _holdController = new L2ShaderHoldController();

    private float _lastShaderFadeDiagLog;
    private float _lastQuadSizeDiagLog;
    private bool _loggedHalfSecondCheckpoint;
    private uint _spriteSpinAppRandBaseState;
    private uint _spriteMotionAppRandBaseState;
    private uint _meshAppRandBaseState;








}
