using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect : TimedCompositeEffectBase
{
    [SerializeField] private CompositePrefabPart[] _parts;
    [SerializeField] private float _serverHitLifetimeTailSeconds = 0f;

    [Header("Lifetime")]
    [Tooltip(
        "If enabled: root/part cast-timed lifetime uses MagicCastData.SkillAnimationDuration " +
        "(wall-clock SpAtk until idle) instead of HitTime. For melee skill FX that must end with the swing.")]
    [SerializeField] private bool _matchLifetimeToSkillAnimation;

    [Tooltip("Если включено: не вызывать DestoryEffect по lifetime корня — дочерние префабы (например wh_heal_ca) не уничтожаются вместе с композитом. Только для отладки шейдеров/Hold.")]
    [SerializeField] private bool _skipDestroyCompositeByLifetime;

    [Header("Effect Light")]
    [Tooltip(
        "Spawn FNManagerLight once per composite Play when a part SpawnPart succeeds. " +
        "Not wired to ParticleGroup/ParticleSingle — only CompositePrefabEffect.SpawnPart → StartSpawn.")]
    [SerializeField] private bool _useLight;
    [SerializeField] private LightEffectSetting _lightSettings;

    public event Action<CompositePrefabEffect, EffectResolveContext> StartSpawn;

    public override void CollectReferencedPrefabs(List<UnityEngine.Object> sink)
    {
        base.CollectReferencedPrefabs(sink);
        if (_parts == null || sink == null)
            return;
        for (int i = 0; i < _parts.Length; i++)
        {
            CompositePrefabPart part = _parts[i];
            if (part == null || part.prefab == null)
                continue;
            sink.Add(part.prefab);
            if (part.prefab.gameObject != null)
                sink.Add(part.prefab.gameObject);
        }
    }

    [Inject] IHomeProjectileService _homeProjectiles;
    [Inject] IAnimationManager _animations;

    protected IAnimationManager Animations => RenderingPacketServices.Animations;

    readonly IEffectAttachmentResolver _resolver = new DefaultEffectAttachmentResolver();
    protected EffectResolveContext _context;
    readonly List<PendingCompositePart> _pendingParts = new List<PendingCompositePart>();
    readonly List<CompositePrefabPart> _pendingHitColliderParts = new List<CompositePrefabPart>();
    readonly List<CompositePrefabPart> _pendingAnimationShootParts = new List<CompositePrefabPart>();
    readonly Dictionary<CompositePrefabPart, BaseEffect> _spawnedPartInstances = new Dictionary<CompositePrefabPart, BaseEffect>();
    readonly HashSet<CompositePrefabPart> _launchedProjectileParts = new HashSet<CompositePrefabPart>();
    readonly HashSet<CompositePrefabPart> _launchedHomeProjectileParts = new HashSet<CompositePrefabPart>();
    readonly List<AnimationEventsBase> _shootEventSources = new List<AnimationEventsBase>();
    Coroutine _pendingSpawnRoutine;
    Coroutine _fallbackShootRoutine;
    Coroutine _visibilityProbeRoutine;
    AnimationEventsBase _animationEvents;
    protected float _playStartedAt;
    bool _isSubscribedToAnyShoot;
    bool _isSubscribedToProjectileEffectHit;
    bool _lightSpawned;
    string _lastShootChannel;
    float _lastShootAt = -1f;
    string _lastShootStack;

    protected IHomeProjectileService HomeProjectiles =>
        CompositeHomeProjectileAccess.Resolve(ref _homeProjectiles);

    private void WireRenderingPacketServices()
    {
        RenderingPacketServices.Bind(animations: _animations);
    }

    protected override string DebugPrefix => "[CompositePrefabEffect]";
    protected override float RuntimeLifeTimeTailSeconds => _serverHitLifetimeTailSeconds;
    protected bool SkipDestroyCompositeByLifetime => _skipDestroyCompositeByLifetime;
    protected virtual bool ShouldUseLegacyPartPipeline => true;
    protected virtual bool UseLegacyLifetimeHacks => true;

    protected override float ResolveCastTimedLifetimeSeconds()
    {
        if (_matchLifetimeToSkillAnimation &&
            _castData != null &&
            _castData.SkillAnimationDuration > 0f)
        {
            return _castData.SkillAnimationDuration;
        }

        return base.ResolveCastTimedLifetimeSeconds();
    }

    public override void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {
        WireRenderingPacketServices();
        base.Setup(settings, castData, owner);
        InitializeTimedComposite(settings, castData);
        _context = CompositeEffectUtilities.BuildContext(owner, castData);
        _spawnedPartInstances.Clear();
        _launchedProjectileParts.Clear();
        _launchedHomeProjectileParts.Clear();
        _pendingHitColliderParts.Clear();
        _pendingAnimationShootParts.Clear();
        _lightSpawned = false;
        if (ShouldUseLegacyPartPipeline)
        {
            SubscribeShootEventIfNeeded();
            SubscribeProjectileHitEventIfNeeded();
        }
    }

    public override void Play()
    {
        if (!ShouldUseLegacyPartPipeline)
        {
            PlayV2();
            return;
        }

        if (_parts == null || _parts.Length == 0)
        {
            // Debug.LogWarning("CompositePrefabEffect: no parts configured.");
            return;
        }

        _lightSpawned = false;
        _lastShootChannel = null;
        _lastShootAt = -1f;
        _lastShootStack = null;
        _playStartedAt = Time.time;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Play started at={_playStartedAt:F3}s hit={(_castData != null ? _castData.HitTime : -1f):F3}s " +
            // $"flight={(_castData != null ? _castData.FlightTime : -1f):F3}s serverShoot={(_castData != null ? _castData.serverTimeToShoot : -1f):F3}s.");
#endif

        CompositePartScheduler.Queue(
            _parts,
            _castData,
            _pendingParts,
            _pendingHitColliderParts,
            _pendingAnimationShootParts,
            SpawnPart);
        if (_castData != null && _castData.SpawnShotVisualsImmediately)
            ProcessShootEvent("none-anim");
        StartPendingPartsRoutineIfNeeded();
        StartShootFallbackRoutineIfNeeded();
        if (!_skipDestroyCompositeByLifetime)
        {
            DestroyCompositeByLifetime();
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            // Debug.Log($"{DebugPrefix} SkipDestroyCompositeByLifetime=true — корень композита не будет уничтожен по lifetime (дочерние объекты остаются в сцене).");
        }
#endif
    }

}
