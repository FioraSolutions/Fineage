using UnityEngine;

public sealed partial class EntityActionMove : IEntityActionProcess
{
    public EntityActionMove(L2PawnRange pawnRange, L2PawnYaw pawnYaw)
    {
        _pawnRange = pawnRange;
        _pawnYaw = pawnYaw;
    }

    public void Enter(Entity entity, object payload)
    {
        if (entity == null || entity.IsDead() || ControllerPacketServices.Moves == null)
            return;

        if (payload is CharMoveToLocationDto moveDto)
        {
            L2PawnRange.TrySnapUserToPacket(entity, moveDto.OldPosition, "MoveTo origin");
            StartMoveToPoint(entity, moveDto.NewPosition);
            return;
        }

        if (payload is Vector3 point)
        {
            StartMoveToPoint(entity, point);
            return;
        }

        if (payload is MoveToPawnDto pawnDto)
            StartMoveToPawn(entity, pawnDto);
    }
}
