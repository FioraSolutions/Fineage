using System;
using UnityEngine;
using System.Collections.Generic;

public sealed partial class WorldPacketApply
{
    public void Die(DieDto dto)
    {
        Entity entity = GetEntity(dto.ObjectId);
        // ReplayAttackTrace.Write("DIE_PACKET", entity, null, "objectId=" + dto.ObjectId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;

        // EntityActionCombatLog.LogIfWatch(entity,
            // "packet Die name=" + EntityActionCombatLog.NameOf(entity) +
            // " id=" + dto.ObjectId +
            // " dead=" + entity.IsDead() +
            // " type=" + entity.GetType().Name);
        apply.OnDie(entity, dto);
        _gameEvents?.Publish(new EntityDiedEvent(dto.ObjectId));
        // UnityChainLog.OnDie(entity, dto);
        // UnityReplaySession.OnDie(entity);
    }

    public void Revive(ReviveDto dto)
    {
        if (dto == null)
            return;

        Entity entity = GetEntity(dto.ObjectId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;

        apply.OnRevive(entity, dto);
    }

    public void Attack(AttackDto dto)
    {
        if (GameWorld == null)
            return;

        Entity target = GameWorld.GetEntityNoLockSync(dto.TargetId);
        Entity attacker = GameWorld.GetEntityNoLockSync(dto.AttackerObjId);
        // if (NpcCombatTrace.IsCombatNpc(attacker))
            // NpcCombatTrace.Write(attacker.IsDead() || target == null ? "ATTACK_SKIP" : "ATTACK",
                // attacker, target,
                // NpcCombatTrace.Place(attacker) + " damage=" + dto.Damage + " flags=" + dto.Flags);
        // ReplayAttackTrace.Write(
            // "ATTACK_PACKET",
            // attacker,
            // target,
            // "damage=" + dto.Damage + " flags=" + dto.Flags +
            // " attackerId=" + dto.AttackerObjId + " targetId=" + dto.TargetId);
        // ReplayAttackTrace.LogHit(attacker, target, dto);
        // EntityActionCombatLog.LogIfWatch(attacker, target,
            // "packet Attack attacker=" + EntityActionCombatLog.Describe(attacker) +
            // " target=" + EntityActionCombatLog.Describe(target) +
            // " attackerDead=" + (attacker != null && attacker.IsDead()) +
            // " targetDead=" + (target != null && target.IsDead()) +
            // " attackerAction=" + (attacker != null ? attacker.ActionSlot.Action.ToString() : "none"));
        EntityWorldApply apply;
        if (attacker == null || !TryResolve(attacker, out apply))
            return;

        // UnityReplaySession.OnAttack(attacker, dto);
        apply.OnAttack(attacker, target, dto);
        // UnityChainLog.OnAttack(attacker, target, dto);
    }

    public void AutoAttackStart(AutoAttackStartDto dto)
    {
        if (dto == null)
            return;
        Entity entity = GetEntity(dto.EntityId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;
        // if (NpcCombatTrace.IsCombatNpc(entity))
            // NpcCombatTrace.Write("ATK_START", entity, null, NpcCombatTrace.Place(entity));
        // EntityActionCombatLog.LogIfWatch(entity,
            // "packet AutoAttackStart name=" + EntityActionCombatLog.NameOf(entity) +
            // " id=" + dto.EntityId);
        apply.OnAutoAttackStart(entity, dto);
        // UnityChainLog.OnAutoAttackStart(entity, dto);
        // UnityReplaySession.OnAttackStart(entity);
    }

    public void AutoAttackStop(AutoAttackStopDto dto)
    {
        if (dto == null)
            return;
        Entity entity = GetEntity(dto.EntityId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;
        // if (NpcCombatTrace.IsCombatNpc(entity))
            // NpcCombatTrace.Write("ATK_STOP", entity, null, NpcCombatTrace.Place(entity));
        // EntityActionCombatLog.LogIfWatch(entity,
            // "packet AutoAttackStop name=" + EntityActionCombatLog.NameOf(entity) +
            // " id=" + dto.EntityId +
            // " inCombat=" + entity.InCombat);
        apply.OnAutoAttackStop(entity, dto);
        // UnityChainLog.OnAutoAttackStop(entity, dto);
        // UnityReplaySession.OnAttackStop(entity);
    }
}
