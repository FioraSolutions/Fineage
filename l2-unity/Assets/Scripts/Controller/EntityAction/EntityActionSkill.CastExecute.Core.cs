using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill
{
    static void BeginVisualLock(Entity entity, MagicSkillUseDto magic)
    {
        UserEntity user = entity as UserEntity;
        if (user == null)
            return;

        float durationSec = magic != null && magic.HitTime > 0
            ? magic.HitTime / 1000f
            : 0.4f;
        user.BeginAttackVisual(durationSec);
    }

    static void ExecutePhysicalSkill(Entity entity, MagicSkillUseDto magic)
    {
        PhysicalSkillPresentation.TryPlay(entity, magic, ResolveEvents(entity));
        // skillgrp desc, otherwise the skill.usk row. No shared melee swing when the combo is missing.
        SkillVisualEffectDispatch.PlayIfKnown(entity, magic, GetCombo(magic));
        ArmPhysicalSkillHit(entity, magic);
    }

    static void ArmPhysicalSkillHit(Entity entity, MagicSkillUseDto magic)
    {
        var hits = CombatPacketServices.Hits;
        if (hits == null || entity == null || magic == null)
            return;

        Entity target = entity.ActionSlot.Target;
        if (target == null && ControllerPacketServices.GameWorld != null)
            target = ControllerPacketServices.GameWorld.GetEntityNoLockSync(magic.TargetId);
        if (target == null || target == entity || target.IsDead())
            return;

        EntityActionPendingDeath.ArmSkillHit(target, magic.HitTime);
        hits.ArmRemoteMeleeHit(entity, target, magic);
    }
}
