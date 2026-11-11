using UnityEngine;

public sealed partial class HomeProjectileMover
{
    public void Launch(
        BaseEffect effect,
        EffectResolveContext context,
        CompositeHomeProjectileConfig config,
        HomeProjectileFlightCoordinator coordinator,
        ParticleGroupHomeFlightProfile groupProfile,
        IEffectAttachmentResolver attachmentResolver)
    {
        if (context == null || context.CasterTransform == null || config == null)
        {
            enabled = false;
            return;
        }

        _attachmentResolver = attachmentResolver;
        _effect = effect;
        _coordinator = coordinator;
        _context = context;
        _homeAttachmentPoint = config.homeAttachmentPoint;
        _homeOffset = config.homeOffset;
        _speed = Mathf.Max(0.01f, config.speed * Mathf.Max(0.01f, groupProfile.speedScale));
        _acceleration = Mathf.Max(0f, config.acceleration);
        _maxSpeed = Mathf.Max(0f, config.maxSpeed);
        _fadeStartDistance = Mathf.Max(0.01f, config.fadeStartDistance);
        _fadeOutSeconds = Mathf.Max(0.05f, config.fadeOutSeconds);
        _arriveDistance = Mathf.Max(0.01f, config.arriveDistance);
        _maxLifetime = config.maxLifetime;
        _arrivalFadeStarted = false;
        _pathCompleted = false;
        _fadeCompleteAt = -1f;
        _rotateToVelocity = config.rotateToVelocity;
        _destroyOnArrive = config.destroyOnArrive;
        _path.Configure(config, groupProfile);
        _path.caster = context.CasterTransform;
        _startPosition = transform.position;
        _pathDistanceEstimate = _path.Estimate(_startPosition, ResolveHomePosition());
        _traveledDistance = 0f;
        _startedAt = Time.time;
        _isLaunched = true;

        CacheEffectParts();
        DetachFromFollow();
        SyncOwnerWorldPosOverride();
        enabled = true;
    }

    void CacheEffectParts()
    {
        _effectParts = GetComponentsInChildren<EffectPart>(true);
    }

    void DetachFromFollow()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null, true);
        }
    }
}
