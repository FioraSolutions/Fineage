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
            EffectManager effects = ControllerPacketServices.Effects;
            if (magic.SkillId == 3 && effects != null)
            {
                effects.PlayEffectSyncedToSkillAnimation(
                    magic.SkillId, entity, magic.HitTime, combo);
            }
            ArmPhysicalSkillHit(entity, magic);
            return;
        }

        EntityActionVisual.PlayPhysicalSkill(entity, magic);
        PlaySkillEffect(entity, magic);
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

        hits.ArmRemoteSkillHit(entity, target, magic);
    }
}
