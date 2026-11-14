using UnityEngine;

public partial class MovementData
{
    private Vector3 ApplyCharInfoGravityAndGround(Vector3 position, float deltaTime, out bool grounded)
    {
        // GroundMask is only Terrain | StaticMesh | Brush. Orc-camp sheets often
        // sit on Default, so a SphereCast on that mask misses and the old gravity
        // branch walked the pawn through the mesh. KeepOnBrush uses the same
        // walkable layers as spawn and does not free-fall on a miss.
        Vector3 stood = WorldPlacement.KeepOnBrush(position);
        _verticalVelocity = 0f;
        grounded = Mathf.Abs(stood.y - position.y) <= 0.05f || stood.y > position.y;
        return stood;
    }

    public void Move(Vector3 direction , float speed)
    {
        Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
        // Mobius moves monsters on the geo segment (getValidLocation), not against a mesh.
        // CharacterController.Move stops on the visual wall and the walk cycle keeps playing.
        if (L2PawnWalkEntity.IsCharInfoPawn(_entity) || _entity is MonsterEntity || _entity is NpcEntity)
        {
            ReleasePawnController();
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
