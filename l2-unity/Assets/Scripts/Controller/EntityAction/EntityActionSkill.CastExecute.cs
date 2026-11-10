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

    static void ExecuteMagicSkill(Entity entity, MagicSkillUseDto magic, Entity target)
    {
        AnimationCombo combo = GetCombo(magic);
        string raceName = entity.RaceId.ToString();
        if (magic.SkillId == 4247)
        {
            Debug.Log(
                "[WINDSTRIKE] ExecuteMagic combo=" + (combo != null) +
                " executor=" + (SkillExecutorPacketServices.Executor != null) +
                " skillGrp=" + (magic.SkillGrp != null) +
                " race=" + raceName);
        }
        if (combo == null || SkillExecutorPacketServices.Executor == null || magic.SkillGrp == null)
        {
            MagicGaitTrace.Event(entity, "ExecuteMagic-FALLBACK-phys",
                "skill=" + magic.SkillId + " race=" + raceName +
                " combo=" + (combo != null) + " skillGrp=" + (magic.SkillGrp != null));
            PlaySkillEffect(entity, magic);
            EntityActionVisual.PlayPhysicalSkill(entity, magic);
            return;
        }
        MagicGaitTrace.Event(entity, "ExecuteMagic-override",
            "skill=" + magic.SkillId + " race=" + raceName + " hitTime=" + magic.HitTime);
        // Debug.Log("[MAGIC_SHOT_LOG] CAST override nick=" + EntityActionCombatLog.NameOf(entity) +
            // " skill=" + magic.SkillId +
            // " raceId=" + raceName +
            // " hitTime=" + magic.HitTime);

        int objectId = entity.Identity.Id;
        string[] orderedCycle = SetupDurationHelper.BuildOrderedCycleForOverrideTiming(combo.GetAnimCycle());
        float[] durations = ControllerPacketServices.Animations != null
            ? ControllerPacketServices.Animations.GetOverrideClipsDurations(objectId, orderedCycle)
            : System.Array.Empty<float>();
        float shotEventTime = ControllerPacketServices.Animations != null
            ? SetupDurationHelper.ResolveShotEventTime(objectId, orderedCycle)
            : 0f;
        Transform targetTransform = target != null ? target.transform : entity.AttackTarget;
        float flightTimeMs = SetupDurationHelper.ResolveMagicFlightTimeMs(entity, magic.SkillId, targetTransform);
        entity.SetupTotalCastDuration(magic.HitTime, flightTimeMs, durations, shotEventTime, magic.TargetId, SetupDurationHelper.CastStyleOf(magic), SetupDurationHelper.MixComboOf(magic));

        _ = SkillExecutorPacketServices.Executor.ExecuteSkillOverride(magic.SkillGrp, entity, combo, ResolveEvents(entity));
    }

    static void ExecuteSelfSkill(Entity entity, MagicSkillUseDto magic)
    {
        if (SetupDurationHelper.IsUsePotion(magic))
        {
            PlaySkillEffect(entity, magic);
            return;
        }

        if (IsMagicSkill(magic))
        {
            AnimationCombo combo = GetCombo(magic);
            if (combo != null && SkillExecutorPacketServices.Executor != null)
            {
                int objectId = entity.Identity.Id;
                SetupDurationHelper.SetupDurationIfHitTimeNot0(magic, objectId, entity, combo);
                _ = SkillExecutorPacketServices.Executor.ExecuteSkillOverride(
                    magic.SkillGrp, entity, combo, ResolveEvents(entity));
                return;
            }
        }

        if (SetupDurationHelper.TryGetPlayableCombo(magic, out AnimationCombo castCombo) &&
            magic.SkillGrp != null &&
            SkillExecutorPacketServices.Executor != null)
        {
            SetupDurationHelper.SetupDurationIfHitTimeNot0(
                magic, entity.Identity.Id, entity, castCombo);
            _ = SkillExecutorPacketServices.Executor.ExecuteSkillOverride(
                magic.SkillGrp, entity, castCombo, ResolveEvents(entity));
            return;
        }

        Skillgrp skillgrp = magic.SkillGrp;
        if (skillgrp != null && SkillExecutorPacketServices.Executor != null)
        {
            AnimationCombo combo = new AnimationCombo(
                "-1", new[] { skillgrp.GetAnimOperationType3() }, "");
            _ = SkillExecutorPacketServices.Executor.ExecuteSkill(entity, combo, ResolveEvents(entity));
            return;
        }

        PlaySkillEffect(entity, magic);
    }

    static void PlaySkillEffect(Entity entity, MagicSkillUseDto magic)
    {
        bool windstrike = magic != null && magic.SkillId == 4247;
        EffectManager effects = ControllerPacketServices.Effects;
        if (effects == null || entity == null || magic == null)
        {
            if (windstrike)
            {
                Debug.Log(
                    "[WINDSTRIKE] PlaySkillEffect abort effects=" + (effects != null) +
                    " entity=" + (entity != null) +
                    " magic=" + (magic != null));
            }
            return;
        }

        MagicCastData cast = entity.GetMagicCastData();
        if (windstrike)
        {
            Debug.Log(
                "[WINDSTRIKE] PlaySkillEffect id=4247 cast=" + (cast != null) +
                " immediate=" + (cast != null && cast.SpawnShotVisualsImmediately) +
                " targetId=" + (cast != null ? cast.TargetObjectId : 0));
        }
        effects.PlayEffect(magic.SkillId, entity.transform, cast);
        if (windstrike)
            Debug.Log("[WINDSTRIKE] PlaySkillEffect returned");
    }

    static bool TryPlayNoneAnimWorldPawnEffect(Entity entity, MagicSkillUseDto magic)
    {
        bool windstrikeGate = magic != null && magic.SkillId == 4247;
        if (windstrikeGate)
        {
            Debug.Log(
                "[WINDSTRIKE] TryPlay monster=" + IsWorldNpcOrMonster(entity) +
                " anim='" + (magic.SkillGrp != null ? magic.SkillGrp.Animation : "skillgrp-null") +
                "' none=" + IsSkillAnimationNone(magic) +
                " npcId=" + (entity.Identity != null ? entity.Identity.NpcId : 0));
        }

        if (!IsWorldNpcOrMonster(entity) || !IsSkillAnimationNone(magic))
            return false;

        Entity target = ResolveEntity(magic.TargetId);
        float flightTimeMs = SetupDurationHelper.ResolveMagicFlightTimeMs(
            entity,
            magic.SkillId,
            target != null ? target.transform : entity.AttackTarget);
        entity.SetupTotalCastDuration(
            magic.HitTime,
            flightTimeMs,
            Array.Empty<float>(),
            0f,
            magic.TargetId,
            SetupDurationHelper.CastStyleOf(magic),
            SetupDurationHelper.MixComboOf(magic));

        MagicCastData cast = entity.GetMagicCastData();
        int npcId = entity.Identity != null ? entity.Identity.NpcId : 0;
        string sequence;
        bool hasNpcOverride = TablePacketServices.MobSkillAnimGrps.TryGetSequence(
            npcId, magic.SkillId, out sequence);
        if (hasNpcOverride)
            sequence = ResolvePlayableNpcSkillSequence(entity, sequence);

        if (hasNpcOverride)
            EntityActionVisual.CancelMove(entity);

        if (cast != null)
            cast.SpawnShotVisualsImmediately = !hasNpcOverride;

        PlaySkillEffect(entity, magic);


        if (hasNpcOverride &&
            ControllerPacketServices.Animations != null &&
            entity.Identity != null)
        {
            // MobSkillLog.Play(entity, sequence);
            ControllerPacketServices.Animations.PlayMonsterAnimation(
                entity.Identity.Id, sequence);
        }

        return true;
    }

}
