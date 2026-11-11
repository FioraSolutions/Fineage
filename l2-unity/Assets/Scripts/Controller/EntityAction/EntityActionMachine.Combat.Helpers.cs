using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public sealed partial class EntityActionMachine
{
    static void CollectAttacker(Entity dead, Entity attacker, List<Entity> attackers)
    {
        if (attacker == null || attacker == dead || attacker.IsDead())
            return;
        if (!IsTargeting(attacker, dead))
            return;
        if (!attackers.Contains(attacker))
            attackers.Add(attacker);
    }

    static bool IsTargeting(Entity attacker, Entity dead)
    {
        if (attacker.ActionSlot.Target == dead)
            return true;
        if (attacker.AttackTarget != null && dead != null && dead.transform != null)
            return attacker.AttackTarget == dead.transform;
        return false;
    }

    public void StopAttack(Entity entity)
    {
        if (!IsAllowed(entity) || entity.IsDead())
            return;

        entity.InCombat = false;
        RefreshStandWaitIfIdle(entity, "AutoAttackStop");
    }

    public void StartAttackStance(Entity entity)
    {
        if (!IsAllowed(entity) || entity.IsDead())
            return;

        entity.InCombat = true;
        RefreshStandWaitIfIdle(entity, "AutoAttackStart");
    }

    void RefreshStandWaitIfIdle(Entity entity, string reason)
    {
        EntityActionKind action = entity.ActionSlot.Action;
        bool busy = action == EntityActionKind.Move ||
            IsSkillOrAttack(action) ||
            IsFinishingSwing(entity);
        if (busy)
        {
            // EntityActionCombatLog.LogIfWatch(entity,
                // reason + " FLAG_ONLY name=" + EntityActionCombatLog.NameOf(entity) +
                // " action=" + action);
            // EntityActionCombatLog.LogCiPawn(entity,
                // reason + " FLAG_ONLY nick=" + EntityActionCombatLog.NameOf(entity) +
                // " action=" + action +
                // " inCombat=" + entity.InCombat +
                // " pawn=" + EntityActionCombatLog.Describe(EntityPawnLookup.PawnOf(entity)));
            return;
        }

        // EntityActionCombatLog.LogIfWatch(entity,
            // reason + " REFRESH name=" + EntityActionCombatLog.NameOf(entity) +
            // " inCombat=" + entity.InCombat);
        // EntityActionCombatLog.LogCiPawn(entity,
            // reason + " REFRESH nick=" + EntityActionCombatLog.NameOf(entity) +
            // " action=" + action +
            // " inCombat=" + entity.InCombat);
        EntityActionVisual.PlayStandWait(entity);
    }
}
