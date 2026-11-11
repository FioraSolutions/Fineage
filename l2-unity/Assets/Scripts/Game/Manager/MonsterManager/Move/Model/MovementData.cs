using UnityEngine;

public partial class MovementData
{
    private Entity _entity;
    private Transform _transform;
    private L2PawnYawDrive _yawDrive;
    private MovementTarget _movementTarget;

    private float _verticalVelocity = 0;
    private float _gravity = 28;

    // PhysicsVolume.TerminalVelocity (2500 UU/s) scaled to Unity meters (1/52.5),
    // matching PlayerController so falling monsters/NPCs cap identically.
    const float TerminalVelocity = 47.62f;
    private bool _isMove;
    private Vector3 _lastPos;
    private bool _hasLastPos;
    private bool _spdTripStarted;
    private float _spdTripStartTime;
    private float _spdTripDist2d;
    private float _spdLastLogTime;
    private bool _spdLastRunning;
    private string _spdLastGait;
    private bool _spdHasGait;
    private readonly float _locomotionCheckAt;
    private readonly Vector3 _locomotionCheckStart;
    private bool _locomotionChecked;

    const float LocomotionCheckDelay = 0.4f;
    const float LocomotionCheckRadius = 50f;

    // Port of Interlude ULevel::ToFloor / APawn::processLanded ground settle
    // (TraceCallers.h / TraceBodies.h, flags 0x086 / 0x297). The pawn drops a
    // capsule (CollisionRadius, CollisionRadius, CollisionHeight) straight down
    // and its centre rests CollisionHeight above the hit surface.
    const float GroundCastStartAbove = 20f;   // dbl_1F3F9A8 (20.0) spawn-probe lift
    const float GroundCastMaxFall = 300f;      // matches GroundSnapHelper.DefaultMaxDistance

    // Interlude MoveTimer. A new click while still moving keeps this value.
    // Persisted on Entity.LocomotionTimer so the walk->run timer survives
    // MovementData teardown (OnFinish between move packets) instead of resetting
    // to 0 and replaying the 0.4s walk window every packet.
    private float _moveTimer
    {
        get { return _entity != null ? _entity.LocomotionTimer : 0f; }
        set { if (_entity != null) _entity.LocomotionTimer = value; }
    }

    // Local per-MovementData animation cache. NOT persisted on Entity: it must
    // reset with each MovementData so a fresh move always re-plays the locomotion
    // clip (StartMove) instead of being suppressed by a stale cached gait.
    private string _gaitAnim;
    private float _lastForceGaitAt;

    // [GAIT] diagnostics: per-entity log throttle + normalizedTime delta.
    [System.NonSerialized] static readonly System.Collections.Generic.Dictionary<int, float> _syncPrevLogAt =
        new System.Collections.Generic.Dictionary<int, float>();
    [System.NonSerialized] static readonly System.Collections.Generic.Dictionary<int, float> _syncPrevNorm =
        new System.Collections.Generic.Dictionary<int, float>();

    readonly L2PawnYaw _pawnYaw;

    public MovementData(Entity mEntity , MovementTarget movementTarget, L2PawnYaw pawnYaw)
    {
        _entity = mEntity;
        _transform = mEntity != null ? mEntity.transform : null;
        _movementTarget = movementTarget;
        _pawnYaw = pawnYaw;
        _isMove = true;
        _locomotionCheckAt = Time.time + LocomotionCheckDelay;
        _locomotionCheckStart = mEntity != null ? mEntity.transform.position : Vector3.zero;
    }
}
