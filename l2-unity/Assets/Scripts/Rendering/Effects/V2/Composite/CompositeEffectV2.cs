using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V2 composite: thin SerializeReference parts + placement. Legacy ParticleGroup
/// children are hijacked onto ParticleStreamDriver. New ParticleGroupV2 plays directly.
/// </summary>
public partial class CompositeEffectV2 : CompositePrefabEffect
{
    // L2 emitter is X-forward (bDirectional). Unity LookRotation is Z-forward.
    // Keep this on the whole directional actor: Mesh PTRS_Actor and sprite
    // PTDU_Normal ProjectionNormal=(1,0,0) both read owner +X as hit.
    const float HitDirectionYawOffsetDegrees = -90f;
    const float ProjectileImpactLifetimeSafetySeconds = 0.15f;

    [SerializeField]
    [SerializeReference]
    CompositePart[] _v2Parts;

    readonly IEffectAttachmentResolver _v2Resolver = new DefaultEffectAttachmentResolver();
    readonly List<PendingV2Part> _pendingDelayed = new List<PendingV2Part>();
    readonly List<CompositePart> _pendingHitCollider = new List<CompositePart>();
    readonly List<CompositePart> _pendingAnimationShoot = new List<CompositePart>();
    readonly Dictionary<CompositePart, BaseEffect> _spawned = new Dictionary<CompositePart, BaseEffect>();
    readonly HashSet<CompositePart> _launchedProjectiles = new HashSet<CompositePart>();
    readonly List<AnimationEventsBase> _v2ShootSources = new List<AnimationEventsBase>();

    AnimationEventsBase _v2AnimationEvents;
    Coroutine _v2PendingRoutine;
    Coroutine _v2ShootWatchdogRoutine;
    bool _v2SubscribedShoot;
    bool _v2SubscribedHit;
    bool _v2DirectShootObserved;

    protected override string DebugPrefix => "[CompositeEffectV2]";


    public override void CollectReferencedPrefabs(List<UnityEngine.Object> sink)
    {
        base.CollectReferencedPrefabs(sink);
        if (_v2Parts == null || sink == null)
            return;
        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart part = _v2Parts[i];
            if (part == null || part.prefab == null)
                continue;
            sink.Add(part.prefab);
            if (part.prefab.gameObject != null)
                sink.Add(part.prefab.gameObject);
        }
    }

    bool TraceCrasherWind()
    {
        return _owner != null &&
               _owner.name.IndexOf("Crasher", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public override void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {
        base.Setup(settings, castData, owner);
        ClearV2Runtime();
        if (!ShouldUseLegacyPartPipeline)
        {
            SubscribeV2ShootIfNeeded();
            SubscribeV2HitIfNeeded();
        }
    }

    protected override void PlayV2()
    {
        if (!HasSpawnableV2Parts)
        {
            if (TraceCrasherWind())
                Debug.Log("[WINDSTRIKE] Composite no V2 parts");
            return;
        }

        _playStartedAt = Time.time;
        RefreshV2Context();
        LogHomeSpawnPlay();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Play started at={_playStartedAt:F3}s hit={(_castData != null ? _castData.HitTime : -1f):F3}s " +
            // $"flight={(_castData != null ? _castData.FlightTime : -1f):F3}s serverShoot={(_castData != null ? _castData.serverTimeToShoot : -1f):F3}s.");
#endif

        QueueV2Parts();
        if (TraceCrasherWind())
        {
            Debug.Log(
                "[WINDSTRIKE] Composite queued shoot=" + _pendingAnimationShoot.Count +
                " delayed=" + _pendingDelayed.Count +
                " spawned=" + _spawned.Count +
                " immediate=" + (_castData != null && _castData.SpawnShotVisualsImmediately));
        }
        if (_castData != null && _castData.SpawnShotVisualsImmediately)
            SpawnPendingAnimationShootParts();
        if (TraceCrasherWind())
            Debug.Log("[WINDSTRIKE] Composite after shoot spawned=" + _spawned.Count);
        StartV2PendingRoutineIfNeeded();
        StartV2ShootWatchdogIfNeeded();
        if (!SkipDestroyCompositeByLifetime)
        {
            EffectSettings lifeTimeSettings = SelectLifetimeSettings();
            if (lifeTimeSettings != null)
            {
                ExtendLifetimeThroughProjectileImpact(lifeTimeSettings);
                DestoryEffect(lifeTimeSettings, _castData);
            }
        }
    }

    protected override void PreparePartPlayback(CompositePrefabPart part, BaseEffect instance)
    {
        PrepareV2Playback(instance);
    }
}
