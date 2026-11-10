using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public sealed partial class EntityActionMachine
{
    public void Die(Entity entity, bool alreadyCorpse = false, string source = null)
    {
        if (entity == null)
            return;

        string src = string.IsNullOrEmpty(source) ? "machine" : source;

        // EntityActionCombatLog.LogIfWatch(entity,
            // "Die name=" + EntityActionCombatLog.NameOf(entity) +
            // " allowed=" + IsAllowed(entity) +
            // " alreadyCorpse=" + alreadyCorpse +
            // " src=" + src +
            // " inCombat=" + entity.InCombat +
            // " action=" + entity.ActionSlot.Action);

        if (IsAllowed(entity))
        {
            entity.InCombat = false;
            entity.AttackTarget = null;
            if (entity.ActionSlot != null)
                entity.ActionSlot.Target = null;
            EntityActionVisual.CancelMove(entity);
            EntityActionVisual.PlayDeath(entity, alreadyCorpse);
            Set(entity, EntityActionKind.Stop, null);
        }

        StopAttackersOf(entity);
    }

    public void CancelAttackKeepStance(Entity entity)
    {
        if (!IsAllowed(entity) || entity.IsDead())
            return;

        var hits = CombatPacketServices.Hits;
        if (hits != null)
            hits.FlushRemoteMeleeHit(entity);

        EntityActionKind action = entity.ActionSlot.Action;
        entity.InCombat = true;
        entity.AttackTarget = null;
        entity.ActionSlot.Target = null;
        L2PawnRange.ClearIgnoredPawn(entity);
        EntityActionVisual.CancelMove(entity);

        // EntityActionCombatLog.Watch(entity);
        // EntityActionCombatLog.LogIfWatch(entity,
            // "ReleaseDeadTarget keepSwing=" + IsFinishingSwing(entity) +
            // " " + EntityActionCombatLog.Describe(entity) +
            // " fromAction=" + action);
        // EntityActionCombatLog.LogGap(entity, "ReleaseDeadTarget", null,
            // " fromAction=" + action +
            // " keepSwing=" + IsFinishingSwing(entity));

        if (IsFinishingSwing(entity) || action == EntityActionKind.Idle)
            return;

        Set(entity, EntityActionKind.Idle, null);
    }

    public void ApplyStop(Entity entity, StopMoveDto dto, bool skipPositionCorrection = false)
    {
        if (!IsAllowed(entity) || entity.IsDead())
            return;

        EntityActionKind action = entity.ActionSlot.Action;
        bool preserveCombat = IsSkillOrAttack(action) || IsFinishingSwing(entity);

        EntityActionStop.StopLocomotion(entity);
        EntityActionStop.TryCorrectPosition(entity, dto, _pawnRange, skipPositionCorrection || preserveCombat);

        if (preserveCombat)
        {
            // EntityActionCombatLog.LogStep(entity,
                // "STOP KEEP_COMBAT nick=" + EntityActionCombatLog.NameOf(entity) +
                // " action=" + action +
                // " skipPos=" + skipPositionCorrection +
                // " nowL2=" + EntityActionCombatLog.L2(entity.transform.position));
            // EntityActionCombatLog.LogIfWatch(entity,
                // "StopMove keep combat name=" + EntityActionCombatLog.NameOf(entity) +
                // " action=" + action);
            return;
        }

        // EntityActionCombatLog.LogIfWatch(entity,
            // "StopMove→Stop name=" + EntityActionCombatLog.NameOf(entity) +
            // " action=" + action +
            // " inCombat=" + entity.InCombat);
        // EntityActionCombatLog.LogCiPawn(entity,
            // "ApplyStop SET Stop nick=" + EntityActionCombatLog.NameOf(entity) +
            // " action=" + action +
            // " inCombat=" + entity.InCombat +
            // " skipPos=" + skipPositionCorrection);
        Set(entity, EntityActionKind.Stop, dto);
    }

    void StopAttackersOf(Entity dead)
    {
        if (dead == null)
            return;

        List<Entity> attackers = new List<Entity>();
        CollectAttackersOf(dead, _active, attackers);

        World world = ControllerPacketServices.GameWorld;
        if (world != null)
            world.ForEachEntity(entity => CollectAttacker(dead, entity, attackers));

        // if (EntityActionCombatLog.IsWatch(dead) || EntityActionCombatLog.ContainsWatch(attackers))
        {
            string names = "";
            for (int i = 0; i < attackers.Count; i++)
            {
                if (i > 0)
                    names += ", ";
                // names += EntityActionCombatLog.Describe(attackers[i]) +
                    // "(action=" + attackers[i].ActionSlot.Action +
                    // " inCombat=" + attackers[i].InCombat + ")";
            }
            // EntityActionCombatLog.Log(
                // "StopAttackersOf dead=" + EntityActionCombatLog.NameOf(dead) +
                // " count=" + attackers.Count +
                // " active=" + _active.Count +
                // " attackers=[" + names + "]");
        }

        for (int i = 0; i < attackers.Count; i++)
            CancelAttackKeepStance(attackers[i]);
    }

    static void CollectAttackersOf(Entity dead, List<Entity> source, List<Entity> attackers)
    {
        if (source == null)
            return;
        for (int i = 0; i < source.Count; i++)
            CollectAttacker(dead, source[i], attackers);
    }

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
