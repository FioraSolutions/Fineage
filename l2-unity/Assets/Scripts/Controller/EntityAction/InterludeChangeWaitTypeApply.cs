using UnityEngine;

/// <summary>
/// Shared <c>ChangeWaitType</c> apply for the local hero: Mobius carries the settled L2 position,
/// then sit/stand/fake-death pose through <see cref="EntityActionVisual"/>.
/// </summary>
public static class InterludeChangeWaitTypeApply
{
    public static void ApplyLocalHero(Entity entity, ChangeWaitTypeDto dto)
    {
        if (entity == null || dto == null)
            return;

        SettlePosition(entity, dto);

        if (dto.WaitType == WaitType.WT_START_FAKEDEATH)
        {
            LocalCastStop.Halt();
            CastMoveLock.Clear();
            EntityActionVisual.CancelMove(entity);
            EntityActionVisual.PlayWaitType(entity, dto.WaitType);
            return;
        }

        EntityActionVisual.CancelMove(entity);
        EntityActionVisual.PlayWaitType(entity, dto.WaitType);

        if (dto.WaitType == WaitType.WT_STOP_FAKEDEATH)
        {
            PlayerStateMachine machine = PlayerStatePacketServices.PlayerState;
            if (machine != null && machine.State == PlayerState.DEAD)
                machine.ChangeIntention(Intention.INTENTION_IDLE);
        }
    }

    public static void SettlePosition(Entity entity, ChangeWaitTypeDto dto)
    {
        if (entity.Identity == null)
            return;

        entity.Identity.SetServerPosition(dto.Position);
        Vector3 stop = GroundSnapHelper.SnapToGroundOrKeep(dto.Position);
        if (entity is PlayerEntity)
        {
            entity.Identity.Position = stop;
            EntitySpawnShared.ApplyGroundedTransform(entity.gameObject, stop, entity.transform.rotation);
            return;
        }

        L2PawnRange.TrySnapUserToPacket(entity, dto.Position, "ChangeWaitType");
    }
}