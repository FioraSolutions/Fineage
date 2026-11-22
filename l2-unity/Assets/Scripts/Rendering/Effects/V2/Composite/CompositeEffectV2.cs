using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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

    bool _partsFromResolver;

    /// <summary>
    /// Parts built by <c>SkillVisualCatalog</c> for this cast. Skips YAML/folder repair,
    /// which would replace the usk phase, attach, and flight data.
    /// </summary>
    public void AssignResolvedParts(CompositePart[] parts)
    {
        _partsFromResolver = true;
        _v2Parts = parts;
    }

    public int DebugV2PartCount => _v2Parts != null ? _v2Parts.Length : 0;

    /// <summary>
    /// Interlude ShotAction flight stage: true when the effect authors a flying part (projectile or home flight).
    /// Without one the skill has no "flight" — its target art is the impact at hit time (ExplosionAction), so the
    /// cast must not reserve a distance-based flight window before HitTime. Unknown/empty part list → true
    /// (keep the legacy timing).
    /// </summary>
    public bool HasFlightStage
    {
        get
        {
            if (_v2Parts == null || _v2Parts.Length == 0)
                return true;

            for (int i = 0; i < _v2Parts.Length; i++)
            {
                CompositePart part = _v2Parts[i];
                if (part is ShotProjectilePart || part is HomeFlightPart)
                    return true;
            }

            return false;
        }
    }

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

    protected override bool UseLegacyLifetimeHacks => false;

    protected override bool ShouldUseLegacyPartPipeline => !HasSpawnableV2Parts;

    bool HasSpawnableV2Parts
    {
        get
        {
            if (_v2Parts == null)
            {
                return false;
            }

            for (int i = 0; i < _v2Parts.Length; i++)
            {
                if (_v2Parts[i] != null && _v2Parts[i].IsSpawnable)
                {
                    return true;
                }
            }

            return false;
        }
    }





}
