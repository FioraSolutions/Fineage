using UnityEngine;

public sealed partial class HomeProjectileMover : MonoBehaviour
{
    readonly HomeProjectilePath _path = new HomeProjectilePath();
    IEffectAttachmentResolver _attachmentResolver;

    EffectResolveContext _context;
    EffectAttachmentPoint _homeAttachmentPoint;
    Vector3 _homeOffset;
    float _speed;
    float _acceleration;
    float _maxSpeed;
    float _fadeStartDistance;
    float _fadeOutSeconds;
    float _arriveDistance;
    float _maxLifetime;
    bool _rotateToVelocity;
    bool _destroyOnArrive;
    BaseEffect _effect;
    HomeProjectileFlightCoordinator _coordinator;
    EffectPart[] _effectParts;
    Vector3 _startPosition;
    float _pathDistanceEstimate;
    float _traveledDistance;
    float _startedAt;
    bool _isLaunched;
    bool _arrivalFadeStarted;
    bool _pathCompleted;
    float _fadeCompleteAt;

}
