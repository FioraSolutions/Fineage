using UnityEngine;

public partial class MovementData
{
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
}
