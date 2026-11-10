using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public sealed partial class EntityActionMachine
{
    public void Revive(Entity entity)
    {
        if (!IsAllowed(entity))
            return;

        entity.SetDead(false);
        if (entity.Status != null && entity.Status.GetHp() <= 0)
        {
            float maxHp = entity.Stats != null && entity.Stats.MaxHp > 0 ? entity.Stats.MaxHp : 100f;
            entity.Status.SetHp(maxHp);
        }
        EntityActionVisual.CancelMove(entity);
        Set(entity, EntityActionKind.Attack, new ReviveDto());
    }

    public void Social(Entity entity, SocialActionDto dto)
    {
        if (entity == null || dto == null)
            return;
        EntityActionVisual.PlaySocial(entity, dto.ActionId);
    }

    public void ChangeWaitType(Entity entity, ChangeWaitTypeDto dto)
    {
        if (!IsAllowed(entity) || dto == null)
            return;
        if (entity.IsDead() && dto.WaitType != WaitType.WT_STOP_FAKEDEATH)
            return;

        EntityActionVisual.CancelMove(entity);
        EntityActionVisual.PlayWaitType(entity, dto.WaitType);
        if (dto.WaitType == WaitType.WT_STANDING || dto.WaitType == WaitType.WT_STOP_FAKEDEATH)
            Set(entity, EntityActionKind.Idle, null);
        else
        {
            entity.ActionSlot.Write(EntityActionKind.Stop, dto, entity.ActionSlot.Target, entity.ActionSlot.Destination);
            Track(entity, EntityActionKind.Stop);
        }
    }

    public void NotifyArrived(Entity entity)
    {
        if (!IsAllowed(entity) || entity.IsDead())
            return;
        if (IsSkillOrAttack(entity.ActionSlot.Action) || ShouldHoldCombatIdle(entity))
        {
            EntitySpawnShared.LogStriderGait("arrived-ignore", entity, "reason=combat");
            return;
        }

        Entity pawn = EntityPawnLookup.PawnOf(entity);
        if (entity is UserEntity && pawn != null && entity.ActionSlot.PawnDist > 0.01f)
        {
            MagicGaitTrace.Event(entity, "NotifyArrived-pawn",
                "pawnDist=" + entity.ActionSlot.PawnDist.ToString("F2") +
                " toPawn=" + VectorUtils.Distance2D(
                    entity.transform.position, pawn.transform.position).ToString("F2") +
                " → CancelMove+PlayStandWait");
            EntityActionVisual.CancelMove(entity);
            EntityActionVisual.PlayStandWait(entity);
            string pose = entity.InCombat ? "Arrived ATKWAIT" : "Arrived WAIT";
            // EntityActionCombatLog.MarkArrivedPose(pose);
            // EntityActionCombatLog.LogCiPawn(entity,
                // pose +
                // " nick=" + EntityActionCombatLog.NameOf(entity) +
                // " nowToPawn=" + VectorUtils.Distance2D(entity.transform.position, pawn.transform.position).ToString("F2") +
                // " pawnDist=" + entity.ActionSlot.PawnDist.ToString("F2") +
                // " inCombat=" + entity.InCombat +
                // " pawn=" + EntityActionCombatLog.Describe(pawn) +
                // EntityActionCombatLog.ChaseDump(entity, pawn));
            float arrivedToPawn = VectorUtils.Distance2D(entity.transform.position, pawn.transform.position);
            // if (arrivedToPawn >= 2f)
                // EntityActionCombatLog.LogGap(entity, "Arrived KEEP_RUN_FAR", pawn);
            // CharInfoSpeedLog.LogArrive(entity, pose);
            return;
        }

        // EntityActionCombatLog.LogCiPawn(entity,
            // "Arrived→Idle nick=" + EntityActionCombatLog.NameOf(entity) +
            // " now=" + EntityActionCombatLog.Vec(entity.transform.position) +
            // " dest=" + EntityActionCombatLog.Vec(entity.ActionSlot.Destination) +
            // " pawn=" + EntityActionCombatLog.Describe(pawn) +
            // " nowToPawn=" + (pawn != null
                // ? VectorUtils.Distance2D(entity.transform.position, pawn.transform.position).ToString("F2")
                // : "-") +
            // " destToPawn=" + (pawn != null
                // ? VectorUtils.Distance2D(entity.ActionSlot.Destination, pawn.transform.position).ToString("F2")
                // : "-") +
            // " " + EntityActionCombatLog.ClassifyDest(entity.transform.position, pawn));
        Set(entity, EntityActionKind.Idle, null);
    }

    public void Remove(Entity entity)
    {
        if (entity == null || entity.Identity == null)
            return;

        L2PawnRange.ClearIgnoredPawn(entity);
        _activeIds.Remove(entity.Identity.Id);
        _active.Remove(entity);
        entity.ActionSlot.Clear();
    }

    public void Tick()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            Entity entity = _active[i];
            if (entity == null)
            {
                _active.RemoveAt(i);
                continue;
            }

            IEntityActionProcess process;
            if (_processes.TryGetValue(entity.ActionSlot.Action, out process))
                process.Tick(entity);
        }
    }

}
