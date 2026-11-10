using System;
using System.Collections.Generic;
using UnityEngine;

class EntityActionSkill : IEntityActionProcess
{
    readonly L2PawnYaw _pawnYaw;

    public EntityActionSkill(L2PawnYaw pawnYaw)
    {
        _pawnYaw = pawnYaw;
    }

    public void Enter(Entity entity, object payload)
    {
        MagicSkillUseDto earlyMagic = payload as MagicSkillUseDto;
        bool windstrike = earlyMagic != null && earlyMagic.SkillId == 4247;
        if (windstrike)
        {
            Debug.Log(
                "[WINDSTRIKE] Skill.Enter type=" + (entity != null ? entity.GetType().Name : "null") +
                " dead=" + (entity != null && entity.IsDead()) +
                " identity=" + (entity != null && entity.Identity != null));
        }

        if (entity == null || entity.IsDead() || entity.Identity == null)
            return;

        MagicSkillUseDto magic = earlyMagic;
        if (magic == null)
            return;

        if (TryApplyWeaponCharge(entity, magic))
        {
            if (windstrike)
                Debug.Log("[WINDSTRIKE] Skill.Enter stopped at weapon-charge");
            return;
        }

        if (SetupDurationHelper.IsUsePotion(magic))
        {
            if (windstrike)
                Debug.Log("[WINDSTRIKE] Skill.Enter potion");
            PlaySkillEffect(entity, magic);
            return;
        }

        bool wasMoving = entity.Identity != null && ControllerPacketServices.Moves != null &&
            ControllerPacketServices.Moves.IsMoving(entity.Identity.Id);
        MagicGaitTrace.Event(entity, "Skill.Enter",
            "skill=" + magic.SkillId +
            " isMagic=" + IsMagicSkill(magic) +
            " hitTime=" + magic.HitTime +
            " wasMoving=" + wasMoving +
            " target=" + magic.TargetId);
        // if (IsWorldNpcOrMonster(entity))
            // MobSkillLog.Begin(entity, magic.SkillId, wasMoving);
        bool keepMove = IsWorldNpcOrMonster(entity);
        // EntityActionCombatLog.LogStep(entity,
            // "SKILL CANCEL_MOVE nick=" + EntityActionCombatLog.NameOf(entity) +
            // " skill=" + magic.SkillId +
            // " wasMoving=" + wasMoving +
            // " keepMove=" + keepMove +
            // " nowL2=" + EntityActionCombatLog.L2(entity.transform.position) +
            // " destL2=" + EntityActionCombatLog.L2(entity.ActionSlot.Destination));
        if (!keepMove)
            EntityActionVisual.CancelMove(entity);

        Entity target = ControllerPacketServices.GameWorld != null
            ? ControllerPacketServices.GameWorld.GetEntityNoLockSync(magic.TargetId)
            : null;
        bool selfCast = entity.Identity != null && entity.Identity.Id == magic.TargetId;

        if (!selfCast && target != null && target.IsDead())
        {
            if (windstrike)
            {
                Debug.Log(
                    "[WINDSTRIKE] Skill.Enter stopped dead target=" +
                    (target.Identity != null ? target.Identity.Id.ToString() : target.name));
            }
            return;
        }

        if (windstrike)
        {
            Debug.Log(
                "[WINDSTRIKE] Skill.Enter continue monster=" + IsWorldNpcOrMonster(entity) +
                " self=" + selfCast +
                " target=" + (target != null ? target.name : "null") +
                " anim='" + (magic.SkillGrp != null ? magic.SkillGrp.Animation : "skillgrp-null") +
                "' isMagic=" + IsMagicSkill(magic));
        }

        if (!IsWorldNpcOrMonster(entity))
            entity.InCombat = !SetupDurationHelper.IsUsePotion(magic);
        entity.ActionSlot.Target = target;
        if (target != null)
            entity.AttackTarget = target.transform;

        L2PawnRange.TrySnapUserToPacket(entity, magic.AttackerPos, "Skill pos");
        BeginFacing(entity, magic, target, selfCast);
        BeginVisualLock(entity, magic);

        // EntityActionCombatLog.LogCiPawn(entity,
            // "Skill.Enter nick=" + EntityActionCombatLog.NameOf(entity) +
            // " skill=" + magic.SkillId +
            // " lvl=" + magic.SkillLvl +
            // " magic=" + (magic.SkillGrp != null ? magic.SkillGrp.IsMagic.ToString() : "-") +
            // " hitTime=" + magic.HitTime +
            // " self=" + selfCast +
            // " target=" + EntityActionCombatLog.Describe(target));

        if (ControllerPacketServices.Animations != null && entity.Identity != null && magic.HitTime > 0)
            ControllerPacketServices.Animations.SetSpTimeAtk(entity.Identity.Id, magic.HitTime);

        try
        {
            if (TryPlayNoneAnimWorldPawnEffect(entity, magic))
            {
                if (windstrike)
                    Debug.Log("[WINDSTRIKE] branch=none-anim");
                return;
            }

            string branch = selfCast ? "self" : IsMagicSkill(magic) ? "magic" : "physical";
            if (windstrike)
                Debug.Log("[WINDSTRIKE] branch=" + branch);

            if (selfCast)
                ExecuteSelfSkill(entity, magic);
            else if (IsMagicSkill(magic))
                ExecuteMagicSkill(entity, magic, target);
            else
                ExecutePhysicalSkill(entity, magic);
        }
        catch (Exception ex)
        {
            if (windstrike)
                Debug.LogError("[WINDSTRIKE] Skill.Enter exception " + ex);
            throw;
        }
    }

    public void Tick(Entity entity)
    {
        if (entity == null || entity.IsDead())
            return;

        UserEntity user = entity as UserEntity;
        if (user != null && user.IsAttackVisualPlaying())
            return;

        Entity target = entity.ActionSlot.Target;
        if (target == null && entity.AttackTarget != null)
            target = entity.AttackTarget.GetComponent<Entity>();
        if (target != null && target != entity && target.IsDead())
        {
            MagicGaitTrace.Event(entity, "Skill.Tick-deadTarget→CancelAttackKeepStance", null);
            if (EntityActionPacketServices.Actions != null)
                EntityActionPacketServices.Actions.CancelAttackKeepStance(entity);
            return;
        }

        if (user == null || !user.ShouldReturnFromAttack())
            return;

        MagicGaitTrace.Event(entity, "Skill.Tick→Idle",
            "visExpired ClearAttackVisual → Set Idle");
        user.ClearAttackVisual();
        if (entity.Identity != null)
            CombatPacketServices.Facing?.EndFollow(entity.Identity.Id, "skill-tick");
        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Set(entity, EntityActionKind.Idle, null);
    }

    public static bool TryApplyWeaponCharge(Entity entity, MagicSkillUseDto magic)
    {
        if (entity == null || magic == null || !WeaponChargeSkill.IsCharge(magic.SkillId))
            return false;

        if (WeaponChargeSkill.IsSpirit(magic.SkillId))
        {
            entity.IsSpiritshotCharged = true;
            entity.IsSoulshotCharged = false;
        }
        else
        {
            entity.IsSoulshotCharged = true;
            entity.IsSpiritshotCharged = false;
        }

        WeaponChargeSkillPlayback.Play(
            ControllerPacketServices.Effects,
            magic.SkillId,
            entity.GetWeaponTransform(),
            entity.transform);

        return true;
    }

    public static Entity ResolveEntity(int objectId)
    {
        if (objectId == 0)
            return null;

        if (ControllerPacketServices.GameWorld != null)
        {
            Entity fromWorld = ControllerPacketServices.GameWorld.GetEntityNoLockSync(objectId);
            if (fromWorld != null)
                return fromWorld;
        }

        if (DomainPacketServices.LocalPlayer != null &&
            DomainPacketServices.LocalPlayer.Identity != null &&
            DomainPacketServices.LocalPlayer.Identity.Id == objectId)
            return DomainPacketServices.LocalPlayer;

        return null;
    }

    public static MagicCastData ResolveCastData(int objectId)
    {
        Entity entity = ResolveEntity(objectId);
        return entity != null ? entity.GetMagicCastData() : null;
    }

    public static bool IsLocalPlayer(int objectId)
    {
        return DomainPacketServices.LocalPlayer != null &&
            DomainPacketServices.LocalPlayer.Identity != null &&
            DomainPacketServices.LocalPlayer.Identity.Id == objectId;
    }

    public static void FinishRemoteCast(int objectId)
    {
        if (IsLocalPlayer(objectId))
            return;

        Entity entity = ResolveEntity(objectId);
        if (entity == null)
            return;

        CombatPacketServices.Facing?.EndFollow(objectId, "skill-smb-idle");
        UserEntity user = entity as UserEntity;
        if (user != null)
            user.ClearAttackVisual();

        bool moving = entity.Identity != null && ControllerPacketServices.Moves != null &&
            ControllerPacketServices.Moves.IsMoving(entity.Identity.Id);
        bool moveAction = entity.ActionSlot != null && entity.ActionSlot.Action == EntityActionKind.Move;

        MagicGaitTrace.BeginPostShotWatch(entity);
        MagicGaitTrace.Event(entity, "FinishRemoteCast",
            "moving=" + moving + " moveAction=" + moveAction +
            " → " + (moving || moveAction
                ? (moving ? "ReplayUserGait" : "KEEP action=Move")
                : "Set Idle"));

        if (moving || moveAction)
        {
            if (moving)
            {
                MovementData move = ControllerPacketServices.Moves.GetMove(entity.Identity.Id);
                if (move != null)
                    move.ReplayUserGait();
            }
            return;
        }

        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Set(entity, EntityActionKind.Idle, null);
    }

}
