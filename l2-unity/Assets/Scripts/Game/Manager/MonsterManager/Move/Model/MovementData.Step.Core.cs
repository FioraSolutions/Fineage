using UnityEngine;
using System;

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
}
