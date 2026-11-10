using UnityEngine;

public sealed partial class WorldPacketApply
{
    public void MoveTo(int objId, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
        Entity entity = GetEntity(objId);
        if (entity == null || string.Equals(entity.name, "Elpy"))
            return;

        // if (NpcCombatTrace.IsCombatNpc(entity))
            // NpcCombatTrace.Write(entity.IsDead() ? "MOVE_SKIP" : "MOVE", entity, null,
                // NpcCombatTrace.Place(entity) +
                // " dest=" + destination.x.ToString("F0") + "," + destination.y.ToString("F0") + "," + destination.z.ToString("F0"));

        EntityWorldApply apply;
        if (TryResolve(entity, out apply))
            apply.OnMoveTo(entity, destination, current, dto);
        // UnityReplaySession.OnMoveTo(entity, destination);
    }

    /// <summary>Authoritative snap from zone ValidatePosition drift — not a MoveTo order.</summary>
    public void ApplyPositionCorrection(int objId, Vector3 position)
    {
        Entity entity = GetEntity(objId);
        if (entity == null)
            return;

        EntityWorldApply apply;
        if (TryResolve(entity, out apply))
            apply.OnPositionCorrection(entity, position);
        else
            EntityWorldApply.SnapTransform(entity, position);
    }

    public void StopMove(StopMoveDto dto)
    {
        if (dto == null)
            return;

        Entity entity = GetEntity(dto.ObjId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;

        // if (NpcCombatTrace.IsCombatNpc(entity))
            // NpcCombatTrace.Write("STOP", entity, null, NpcCombatTrace.Place(entity));

        // ReplayAttackTrace.LogStop(entity, dto);
        apply.OnStopMove(entity, dto);
        // UnityChainLog.OnStopMove(entity, dto);
        // UnityReplaySession.OnStopMove(entity, dto);
    }

    public void MoveToPawn(MoveToPawnDto dto)
    {
        Entity entity = GetEntity(dto.ObjId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;

        // if (NpcCombatTrace.IsCombatNpc(entity))
        {
            Entity pawn = GetEntity(dto.TarObjid);
            // NpcCombatTrace.Write(entity.IsDead() ? "PAWN_SKIP" : "PAWN", entity, pawn,
                // NpcCombatTrace.Place(entity) + " pawnId=" + dto.TarObjid + " distUu=" + dto.DistanceUu);
        }

        // ReplayAttackTrace.LogPawn(entity, GetEntity(dto.TarObjid), dto.DistanceUu);
        apply.OnMoveToPawn(entity, dto);
        // UnityChainLog.OnMoveToPawn(entity, dto);
        // UnityReplaySession.OnMoveToPawn(entity, dto);
    }
}
