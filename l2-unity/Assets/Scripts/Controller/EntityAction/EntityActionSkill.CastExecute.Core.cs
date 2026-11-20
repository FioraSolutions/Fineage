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
        AnimationCombo combo = GetCombo(magic);
        if (combo != null && SkillExecutorPacketServices.Executor != null)
        {
            _ = SkillExecutorPacketServices.Executor.ExecuteSkill(entity, combo, ResolveEvents(entity));
            // Physical skills carry their art on the skill itself (Deadly Blow 263 → viz_263,
            // Lethal Blow 344 → viz_344). Only skill 3 used to play here, so every other drop
            // skill animated the swing and showed no effect at all. Syncing to the combo keeps the
            // art on the weapon at the hit moment instead of firing it at cast start.
            SkillVisualEffectDispatch.PlayIfKnown(entity, magic, combo);
            ArmPhysicalSkillHit(entity, magic);
            return;
        }

        EntityActionVisual.PlayPhysicalSkill(entity, magic);
        SkillVisualEffectDispatch.PlayIfKnown(entity, magic);
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
