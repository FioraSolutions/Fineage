using UnityEngine;
using System;

public sealed partial class UserWorldApply : EntityWorldApply
{
    public UserWorldApply(EntityActionMachine actions)
    {
        _actions = actions;
    }

    public override void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
        if (entity == null || entity.IsDead())
            return;

        // EntityPawnLookup.ResolvePawn scanned the whole world per packet for debug logs.
        // Interlude AddMoveTo does not. Keep the scan out of the live move path.
        // bool holdIdle = EntityActionMachine.ShouldHoldCombatIdle(entity);
        // Entity pawn = EntityPawnLookup.ResolvePawn(entity, destination);
        // EntityActionCombatLog.LogCiPawn(entity,
            // "MoveTo nick=" + EntityActionCombatLog.NameOf(entity) +
            // " action=" + entity.ActionSlot.Action +
            // " inCombat=" + entity.InCombat +
            // " holdIdle=" + holdIdle +
            // " distToDest=" + VectorUtils.Distance2D(entity.transform.position, destination).ToString("F2") +
            // " dest=" + EntityActionCombatLog.Vec(destination) +
            // " pawn=" + EntityActionCombatLog.Describe(pawn) +
            // " distToPawn=" + (pawn != null
                // ? VectorUtils.Distance2D(entity.transform.position, pawn.transform.position).ToString("F2")
                // : "-") +
            // " " + EntityActionCombatLog.ClassifyDest(destination, pawn) +
            // " APPLY");
        // EntityActionCombatLog.LogIfWatch(entity,
            // "User.MoveTo chase action=" + entity.ActionSlot.Action +
            // " inCombat=" + entity.InCombat);
        // EntityActionCombatLog.LogStep(entity,
            // "MOVE APPLY nick=" + EntityActionCombatLog.NameOf(entity) +
            // " distM=" + VectorUtils.Distance2D(entity.transform.position, destination).ToString("F2") +
            // " nowL2=" + EntityActionCombatLog.L2(entity.transform.position) +
            // " destL2=" + EntityActionCombatLog.L2(destination) +
            // " action=" + entity.ActionSlot.Action +
            // " inCombat=" + entity.InCombat);
        _actions.Set(entity, EntityActionKind.Move, dto);
    }

    public override void OnStopMove(Entity entity, StopMoveDto dto)
    {
        if (entity != null && entity.IsDead())
            return;
        UserEntity user = entity as UserEntity;
        if (user != null && dto != null)
        {
            float toStop = VectorUtils.Distance2D(entity.transform.position, dto.StopPos);
            float toDest = VectorUtils.Distance2D(entity.transform.position, entity.ActionSlot.Destination);
            Entity pawn = EntityPawnLookup.PawnOf(entity);
            float pawnDist = entity.ActionSlot.PawnDist;
            float nowToPawn = pawn != null
                ? VectorUtils.Distance2D(entity.transform.position, pawn.transform.position)
                : -1f;
            bool chasing = entity.ActionSlot.Action == EntityActionKind.Move ||
                (entity.Identity != null && MoveAllCharactersPacketServices.Moves != null &&
                 MoveAllCharactersPacketServices.Moves.IsMoving(entity.Identity.Id));
            bool skipKeepChase = chasing && pawn != null && pawnDist > 0.01f && nowToPawn > pawnDist + 0.12f;
            bool atDist = pawn != null && pawnDist > 0.01f && nowToPawn >= 0f &&
                nowToPawn <= pawnDist + 0.12f;
            bool skipKeepMove = !skipKeepChase &&
                entity.ActionSlot.Action == EntityActionKind.Move &&
                toStop <= 0.35f &&
                (toDest > 0.5f || atDist) &&
                !(pawn is NpcEntity);
            bool skipPos = skipKeepChase || skipKeepMove;
            string skip = skipKeepChase ? " SKIP_POS_CHASE" : (skipKeepMove ? " SKIP_POS_MOVE" : " APPLY");
            bool moving = entity.Identity != null && MoveAllCharactersPacketServices.Moves != null &&
                MoveAllCharactersPacketServices.Moves.IsMoving(entity.Identity.Id);
            // EntityActionCombatLog.LogStep(entity,
                // "STOP " + skip.Trim() + " nick=" + EntityActionCombatLog.NameOf(entity) +
                // " moving=" + moving +
                // " action=" + entity.ActionSlot.Action +
                // " inCombat=" + entity.InCombat +
                // " toStopM=" + toStop.ToString("F2") +
                // " toDestM=" + toDest.ToString("F2") +
                // " nowL2=" + EntityActionCombatLog.L2(entity.transform.position) +
                // " stopL2=" + EntityActionCombatLog.L2(dto.StopPos) +
                // " destL2=" + EntityActionCombatLog.L2(entity.ActionSlot.Destination));
            // EntityActionCombatLog.LogCiPawn(entity,
                // "StopMove nick=" + EntityActionCombatLog.NameOf(entity) +
                // " action=" + entity.ActionSlot.Action +
                // " inCombat=" + entity.InCombat +
                // " toStop=" + toStop.ToString("F2") +
                // " toDest=" + toDest.ToString("F2") +
                // " pawnDist=" + pawnDist.ToString("F2") +
                // " nowToPawn=" + nowToPawn.ToString("F2") +
                // " stop=" + EntityActionCombatLog.Vec(dto.StopPos) +
                // " dest=" + EntityActionCombatLog.Vec(entity.ActionSlot.Destination) +
                // " pawn=" + EntityActionCombatLog.Describe(pawn) +
                // " stopToPawn=" + (pawn != null
                    // ? VectorUtils.Distance2D(dto.StopPos, pawn.transform.position).ToString("F2")
                    // : "-") +
                // " destToPawn=" + (pawn != null
                    // ? VectorUtils.Distance2D(entity.ActionSlot.Destination, pawn.transform.position).ToString("F2")
                    // : "-") +
                // " " + EntityActionCombatLog.ClassifyDest(dto.StopPos, pawn) +
                // skip);
            // CharInfoMoveBudgetLog.Compare(entity, "STOP" + skip, pawn, dto.StopPos, true);
            // if (nowToPawn >= 2f && !skipPos)
                // EntityActionCombatLog.LogGap(entity, "StopMove APPLY_FAR", pawn,
                    // " toStop=" + toStop.ToString("F2") +
                    // " stop=" + EntityActionCombatLog.Vec(dto.StopPos) +
                    // " stopToPawn=" + (pawn != null
                        // ? VectorUtils.Distance2D(dto.StopPos, pawn.transform.position).ToString("F2")
                        // : "-"));
            _actions.ApplyStop(entity, dto, skipPos);
            return;
        }
        _actions.ApplyStop(entity, dto);
    }
}
