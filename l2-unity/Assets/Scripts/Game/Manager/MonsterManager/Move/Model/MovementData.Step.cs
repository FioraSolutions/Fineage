using UnityEngine;

public partial class MovementData
{
    public bool ReconcileStoppedAnimation()
    {
        if (_entity == null || _entity.IsDead() || _entity.ActionSlot == null)
            return true;

        EntityActionKind action = _entity.ActionSlot.Action;
        if (action == EntityActionKind.Attack ||
            action == EntityActionKind.Skill ||
            action == EntityActionKind.Stop)
        {
            EntitySpawnShared.LogStriderGait("reconcile-skip", _entity, "reason=action");
            return false;
        }

        NetworkAnimationController controller = _entity.GetAnimatorController();
        Animator animator = controller != null ? controller.GetAnimator() : null;
        if (animator == null)
            return true;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        bool staleLocomotion = state.IsName("walk") || state.IsName("run");
        if (!staleLocomotion)
        {
            EntitySpawnShared.LogStriderGait("reconcile-keep", _entity, "reason=not-run-or-walk");
            return true;
        }

        EntitySpawnShared.LogStriderGait("reconcile-stop", _entity, "to=wait");

        // Debug.Log(
            // "[MOVE_ANIM_RECONCILE] nick=" + EntityActionCombatLog.NameOf(_entity) +
            // " objectId=" + (_entity.Identity != null ? _entity.Identity.Id : 0) +
            // " action=" + action +
            // " state=" + (state.IsName("run") ? "run" : "walk") +
            // " moving=false -> idle");

        if (action == EntityActionKind.Move && EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Set(_entity, EntityActionKind.Idle, null);
        else
            EntityActionVisual.PlayStandWait(_entity);
        return true;
    }

    public void StepCharInfo(Vector3 destination, float speed)
    {
        Transform t = GetTransform();
        if (t == null)
            return;

        Vector3 location = t.position;
        Vector3 look = destination - location;
        look.y = 0f;
        bool moved = L2PawnWalk.TryStep(location, destination, speed, Time.deltaTime, out Vector3 stepped);
        if (!moved)
            stepped = location;

        // Interlude physWalking keeps Velocity.Z == 0 while grounded, then
        // processLanded / ToFloor settle the pawn onto the floor under it. We
        // reproduce that split: XY move first, then gravity + a downward capsule
        // probe to snap Y. Without this CharInfo players float / spawn underground.
        stepped = ApplyCharInfoGravityAndGround(stepped, Time.deltaTime, out _);

        LookAlong(t, look);

        Quaternion rot = t.rotation;
        bool rotated = _yawDrive != null && _yawDrive.TryConsumeStep(Time.deltaTime, out rot);
        if (moved && rotated)
            t.SetPositionAndRotation(stepped, rot);
        else if (moved)
            t.position = stepped;
        else if (rotated)
            t.rotation = rot;

        CheckLocomotionAnimation();
    }

    /// </summary>
    /// bounce between two different ground conventions.
    /// GroundSnapHelper / spawn / NetworkTransformReceive so the pawn does not
    /// floor. Y is written as the surface height exactly (hit.point.y), matching
    /// probe). Applies gravity to _verticalVelocity, then rests the pawn on the
    /// ULevel::ToFloor / APawn::processLanded (0x086 / 0x297 downward capsule
    /// Port of Interlude APawn::physWalking (planar velocity + gravity) and
    /// <summary>
    private Vector3 ApplyCharInfoGravityAndGround(Vector3 position, float deltaTime, out bool grounded)
    {
        float radius = _entity != null ? _entity.GetCollissionRadius() : 0f;
        if (radius <= 0.001f)
            radius = 0.05f;

        LayerMask mask = GroundSnapHelper.ResolveGroundMask(null);

        // Downward capsule probe starting above the pawn (Interlude ToFloor starts
        // at Location.Z - CollisionHeight, but we probe a safe distance above to
        // catch a pawn that ended up under the terrain surface).
        Vector3 origin = new Vector3(position.x, position.y + GroundCastStartAbove, position.z);
        bool hitFloor = Physics.SphereCast(
            origin, radius, Vector3.down, out RaycastHit hit,
            GroundCastStartAbove + GroundCastMaxFall,
            mask, QueryTriggerInteraction.Ignore);

        float targetY;
        if (hitFloor)
        {
            // Land on the surface. The project-wide ground convention is
            // transform.position.y == hit.point.y (see GroundSnapHelper), so we do
            // not add a capsule half-height here — otherwise the pawn lifts ~30-40cm
            // above the ground each frame vs. its spawn height.
            targetY = hit.point.y;
            _verticalVelocity = 0f;
            grounded = true;
        }
        else
        {
            // No floor below: keep falling (Interlude physFalling branch), capped
            // at terminal velocity like the fixed PlayerController.
            _verticalVelocity -= _gravity * deltaTime;
            if (_verticalVelocity < -TerminalVelocity)
                _verticalVelocity = -TerminalVelocity;
            targetY = position.y + _verticalVelocity * deltaTime;
            grounded = false;
        }

        return new Vector3(position.x, targetY, position.z);
    }

    public void Move(Vector3 direction , float speed)
    {
        Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
        if (L2PawnWalkEntity.IsCharInfoPawn(_entity))
        {
            if (_movementTarget != null && _movementTarget.TryGetTarget(out Vector3 dest))
                StepCharInfo(dest, speed);
            return;
        }

        CharacterMove(_entity, direction, speed);

        CheckLocomotionAnimation();
        LookAlong(GetTransform(), lookDir);
    }

    private void CheckLocomotionAnimation()
    {
        if (_locomotionChecked || Time.time < _locomotionCheckAt)
            return;
        if (!(_entity is MonsterEntity) && !(_entity is NpcEntity))
        {
            _locomotionChecked = true;
            return;
        }

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
            return;

        float distanceToPlayer = VectorUtils.Distance2D(
            _entity.transform.position,
            player.transform.position);
        if (distanceToPlayer > LocomotionCheckRadius)
        {
            _locomotionChecked = true;
            return;
        }

        _locomotionChecked = true;
        string expected = _entity.Running ? "run" : "walk";
        NetworkAnimationController controller = _entity.GetAnimatorController();
        Animator animator = controller != null ? controller.GetAnimator() : null;
        AnimatorStateInfo current = animator != null
            ? animator.GetCurrentAnimatorStateInfo(0)
            : default;
        AnimatorStateInfo next = animator != null && animator.IsInTransition(0)
            ? animator.GetNextAnimatorStateInfo(0)
            : default;
        bool playingExpected = animator != null &&
            (current.IsName(expected) ||
             animator.IsInTransition(0) && next.IsName(expected));
        float moved = VectorUtils.Distance2D(_locomotionCheckStart, _entity.transform.position);

        // Debug.Log(
            // "MONSTER_WALK result=" + (playingExpected ? "OK" : "MISS") +
            // " nick=" + EntityActionCombatLog.NameOf(_entity) +
            // " objectId=" + (_entity.Identity != null ? _entity.Identity.Id : 0) +
            // " expected=" + expected +
            // " actual=" + EntityActionSkill.AnimatorStateName(animator) +
            // " action=" + (_entity.ActionSlot != null ? _entity.ActionSlot.Action.ToString() : "-") +
            // " moving=" + _isMove +
            // " running=" + _entity.Running +
            // " moved=" + moved.ToString("F3") +
            // " distance=" + distanceToPlayer.ToString("F1") +
            // " animator=" + (animator != null) +
            // " enabled=" + (animator != null && animator.enabled) +
            // " speed=" + (animator != null ? animator.speed.ToString("F3") : "-") +
            // " walkBool=" + (animator != null ? animator.GetBool("walk").ToString() : "-") +
            // " runBool=" + (animator != null ? animator.GetBool("run").ToString() : "-") +
            // " hasState=" + (animator != null && animator.HasState(0, Animator.StringToHash(expected))));
    }

    private void CharacterMove(Entity entity , Vector3 direction , float speed)
    {
        CharacterController character = GetControllerToTypeEntity(entity);
        // Match PlayerController: gravity (Y) is integrated separately so the
        // vertical velocity is NOT scaled by run speed. Combining them made the
        // monsters/NPCs bob "light" — the faster they ran, the weaker gravity felt.
        Vector3 planar = new Vector3(direction.x, 0f, direction.z);
        float vy = ApplyGravity(Time.deltaTime);
        StartMove(character, planar, speed, vy);
    }

    private void StartMove(CharacterController character, Vector3 direction, float speed, float verticalVelocity)
    {
        if (character == null || !character.enabled || !character.gameObject.activeInHierarchy)
            return;

        if (character.radius <= 0.001f || character.height <= 0.001f)
            return;

        Vector3 planar = direction * speed * Time.deltaTime;
        planar.y = verticalVelocity * Time.deltaTime;
        character.Move(planar);
    }

    /// </summary>
    /// PhysicsVolume.TerminalVelocity (2500 UU/s → 47.62 m/s).
    /// CharacterController touching the floor. Airborne: gravity capped at
    /// PlayerController.ApplyGravity). Grounded: small downward bias keeps the
    /// Interlude physWalking / physFalling vertical integrator (matches
    /// <summary>
    private float ApplyGravity(float deltaTime)
    {
        var character = GetControllerToTypeEntity(_entity);

        if (character == null)
            return 0f;

        if (character.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity -= _gravity * deltaTime;
            if (_verticalVelocity < -TerminalVelocity)
                _verticalVelocity = -TerminalVelocity;
        }
        return _verticalVelocity;
    }

    public void OnFinish(Vector3 target)
    {
        _isMove = false;
        Vector3 dest;
        if (_movementTarget.IsActorTarget())
        {
            // MoveToPawn: already inside Dist of the pawn. Do not snap to pawn origin
            // (that ran UserEntity into the monster center).
            dest = _entity.transform.position;
        }
        else
        {
            dest = new Vector3(target.x, _entity.transform.position.y, target.z);
        }
        dest = GroundSnapHelper.SnapToGroundOrKeep(dest);
        if (_entity is UserEntity)
            EntitySpawnShared.ApplyGroundedTransform(_entity.gameObject, dest, _entity.transform.rotation);
        else
            _entity.transform.position = dest;
        SetEventToTypeEntity(_entity);
        EntitySpawnShared.LogStriderGait("arrived", _entity);
    }

    public void SetEventToTypeEntity(Entity entity)
    {
        if (entity is PlayerEntity)
            return;

        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.NotifyArrived(entity);
    }
    public CharacterController GetControllerToTypeEntity(Entity entity)
    {
        CharacterController character = null;

        if (entity.GetType() == typeof(MonsterEntity))
        {
            var _mEntity = (MonsterEntity)entity;
            character = _mEntity.GetCharacterController();

        }
        else if (entity.GetType() == typeof(NpcEntity))
        {
            var _mEntity = (NpcEntity)entity;
            character = _mEntity.GetCharacterController();

        }
        else if (entity.GetType() == typeof(UserEntity))
        {
            character = ((UserEntity)entity).GetCharacterController();
        }
        return character;
    }

}
