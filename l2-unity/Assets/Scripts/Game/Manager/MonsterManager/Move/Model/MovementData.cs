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

    public bool IsOwnerDead()
    {
        return _entity != null && _entity.IsDead();
    }

    public static bool IsPlayerPawn(Entity entity)
    {
        return entity is UserEntity || entity is PlayerEntity;
    }

    public void InheritMoveTimer(MovementData prev)
    {
        if (prev == null || !IsPlayerPawn(_entity))
            return;
        // Persist only the walk->run timer. Do NOT copy _gaitAnim: it is a
        // per-MovementData animation cache. Copying it makes SyncUserGait think the
        // clip is already playing and skip StartMove, so point->pawn transitions
        // (and other fresh moves) never re-trigger the run/walk animation.
        _moveTimer = prev._moveTimer;
    }

    /// <summary>
    /// Interlude AddMoveTo: overwrite Destination on the same move, keep timer/gait.
    /// </summary>
    public void RetargetPoint(Vector3 destination, bool running)
    {
        _isMove = true;
        if (_movementTarget == null)
            _movementTarget = new MovementTarget(destination, 0.1f, running);
        else
            _movementTarget.Set(destination, 0.1f, running);
    }

    /// <summary>
    /// Same as RetargetPoint but for MoveToPawn chase packets: keep the existing
    /// MovementData (and its gait cache) instead of tearing it down every packet.
    /// </summary>
    public void RetargetPawn(Transform pawn, float stopDistance, bool running)
    {
        _isMove = true;
        if (_movementTarget == null)
            _movementTarget = new MovementTarget(pawn, stopDistance, running);
        else
            _movementTarget.Set(pawn, stopDistance, running);
    }

    public Entity GetEntity()
    {
        return _entity;
    }

    public bool IsEntity()
    {
        return _entity != null;
    }

    public void SetLastPosition(Vector3 lastPos)
    {
        _lastPos = lastPos;
        _hasLastPos = true;
    }

    public bool HasLastPosition()
    {
        return _hasLastPos;
    }

    public Vector3 GetLastPosition()
    {
        return _lastPos;
    }

    public MovementTarget GetMovementTarget()
    {
        return _movementTarget;
    }

    public Transform GetTransform()
    {
        if (_transform == null && _entity != null)
            _transform = _entity.transform;
        return _transform;
    }

    public float GetDistance()
    {
        return _movementTarget.GetDistance();
    }
    public bool IsMove()
    {
        return _isMove;
    }

    public void SetIsMove(bool isMove)
    {
        _isMove = isMove;
    }

    public float GetSpeed()
    {
        if (_entity.Stats == null)
            return 0f;
        bool npc = !IsPlayerPawn(_entity);
        return L2PlayerMoveGait.Speed(
            _moveTimer,
            _entity.Running,
            npc,
            _entity.Stats.UnitySpeedWalking,
            _entity.Stats.UnitySpeedRun);
    }

    public bool IsWalkGait()
    {
        return L2PlayerMoveGait.IsWalkStart(_moveTimer, _entity.Running, !IsPlayerPawn(_entity));
    }

    public float GetMoveTimer()
    {
        return _moveTimer;
    }

    // True when the cached gait says walk/run but the animator is NOT in (and not
    // fading into) that locomotion state. IMPORTANT: check IsAlreadyPlaying FIRST.
    // During a healthy wait→run CrossFade the current clip is still wait_*, but
    // next=run; treating that as OffGait ForceReplay every frame restarts the fade
    // forever ("half step then freeze") — especially visible on MMagic MoveToPawn
    // after NotifyArrived PlayStandWait.
    bool IsAnimatorOffGait(string wantedGait)
    {
        UserEntity ue = _entity as UserEntity;
        if (ue == null)
            return false;
        NetworkAnimationController nac = ue.GetAnimatorController();
        Animator anm = nac != null ? nac.GetAnimator() : null;
        if (anm == null)
            return false;

        string state = wantedGait + "_" + ue.WeaponAnim;
        if (PlayerLocomotionCrossFade.IsAlreadyPlaying(anm, state))
            return false;

        AnimatorClipInfo[] clips = anm.GetCurrentAnimatorClipInfo(0);
        string clipName = clips != null && clips.Length > 0 && clips[0].clip != null
            ? clips[0].clip.name
            : null;

        // Stuck on a non-locomotion clip with no fade into the wanted gait.
        if (!string.IsNullOrEmpty(clipName))
        {
            bool isWantedLocomotion =
                clipName.IndexOf(".ao_" + wantedGait + "_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("_" + wantedGait + "_", System.StringComparison.OrdinalIgnoreCase) >= 0;
            if (isWantedLocomotion)
                return false;
            if (clipName.IndexOf(".ao_wait_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf(".ao_atkwait_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf(".ao_Wait_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("jatk", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("PicItem", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("pickup", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("MagicShot", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("CastLong", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("CastMid", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("CastEnd", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return true;
    }

    // One-shot forcing replay: guarantees the locomotion clip (re)starts even if
    // the controller/animator reports it as already active or is mid-transition.
    // Used after a direct animator write (e.g. pickup / a no-exit pickup state)
    // which can otherwise leave the pawn frozen while it still translates.
    void LookAlong(Transform t, Vector3 lookDir)
    {
        if (t == null || _pawnYaw == null)
            return;
        int id = _entity.Identity != null ? _entity.Identity.Id : 0;
        if (id != 0 && CombatPacketServices.Facing != null && CombatPacketServices.Facing.HasFollow(id))
        {
            // CombatFacingService.ApplyYaw owns this rotation. A stale drive would
            // overwrite it from StepCharInfo when this Update runs later.
            if (_yawDrive != null)
                _yawDrive.Stop();
            return;
        }
        if (lookDir.sqrMagnitude <= 0.0001f)
            return;
        if (_yawDrive == null)
            _yawDrive = _pawnYaw.EnsureDrive(t);
        _pawnYaw.RequestLook(_yawDrive, t, t.position + lookDir, L2PawnYaw.RotationRateYaw);
    }

   


}
