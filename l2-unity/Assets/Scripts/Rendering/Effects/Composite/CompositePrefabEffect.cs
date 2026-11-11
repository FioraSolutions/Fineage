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


    protected override string DebugPrefix => "[CompositePrefabEffect]";
    protected override float RuntimeLifeTimeTailSeconds => _serverHitLifetimeTailSeconds;
    protected bool SkipDestroyCompositeByLifetime => _skipDestroyCompositeByLifetime;
    protected virtual bool ShouldUseLegacyPartPipeline => true;
    protected virtual bool UseLegacyLifetimeHacks => true;




}
