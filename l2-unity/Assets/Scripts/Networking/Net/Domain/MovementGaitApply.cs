using UnityEngine;

/// <summary>
/// Applies a server ChangeMoveType / MoveState gait to any pawn (player, NPC, summon).
/// Identity.IsRunning and Entity.Running both stay in lockstep so the move pipeline
/// uses the actor's own run/walk, not the local player's toggle.
/// </summary>
public static class MovementGaitApply
{
    public static void Apply(Entity entity, bool running, float runSpeed = 0f, float walkSpeed = 0f)
    {
        if (entity == null || entity.IsDead())
            return;

        if (entity.Identity != null)
            entity.Identity.IsRunning = running;
        entity.UpdateMoveType(running);

        if (runSpeed > 0.001f)
            entity.UpdateRunSpeed(runSpeed);
        if (walkSpeed > 0.001f)
            entity.UpdateWalkSpeed(walkSpeed);
        if (runSpeed <= 0.001f && walkSpeed <= 0.001f)
            entity.RefreshLocomotionAnimRates();

        // Local player locomotion is driven by its own controller; remote pawns already
        // mid-move must retarget so client speed matches the server chase.
        if (entity is UserEntity)
            return;
        if (entity.Identity == null || ControllerPacketServices.Moves == null)
            return;

        MovementData move = ControllerPacketServices.Moves.GetMove(entity.Identity.Id);
        if (move == null || !move.IsMove())
            return;

        move.RetargetPoint(entity.ActionSlot.Destination, running);
        EntityActionVisual.StartMove(entity, !running);
    }
}
