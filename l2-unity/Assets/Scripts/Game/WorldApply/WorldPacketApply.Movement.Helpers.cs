using UnityEngine;
using System;
using System.Collections.Generic;

public sealed partial class WorldPacketApply
{
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
