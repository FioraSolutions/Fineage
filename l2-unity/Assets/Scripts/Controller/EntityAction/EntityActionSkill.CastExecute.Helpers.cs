using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill
{
    static void ExecuteMagicSkill(Entity entity, MagicSkillUseDto magic, Entity target)
    {
        if (magic != null && target != null && target != entity)
            EntityActionPendingDeath.ArmSkillHit(target, magic.HitTime);

        AnimationCombo combo = GetCombo(magic);
        string raceName = entity.RaceId.ToString();
        if (SkillPresentationCatalog.IsWindStrike(magic.SkillId))
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
            SkillVisualEffectDispatch.PlayIfKnown(entity, magic);
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
        float flightTimeMs = SetupDurationHelper.ResolveMagicFlightTimeMs(entity, magic.SkillId, magic.SkillLvl, targetTransform);
        entity.SetupTotalCastDuration(magic.HitTime, flightTimeMs, durations, shotEventTime, magic.TargetId, SetupDurationHelper.CastStyleOf(magic), SetupDurationHelper.MixComboOf(magic), magic.SkillId);

        _ = SkillExecutorPacketServices.Executor.ExecuteSkillOverride(magic.SkillGrp, entity, combo, ResolveEvents(entity));
    }

    static void ExecuteSelfSkill(Entity entity, MagicSkillUseDto magic)
    {
        if (SetupDurationHelper.IsUsePotion(magic))
        {
            SkillVisualEffectDispatch.PlayIfKnown(entity, magic);
            return;
        }

        // Mage self-buffs (is_magic=1): CastMid/CastEnd/MagicNoTarget via override runner.
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

        // Songs/dances (is_magic=3, anim W=SpAtk05) and physical self skills (Sonic Focus X=SpAtk06)
        // play the skillgrp SpAtk phases. A one-clip override would map that gesture to CastEnd.
        AnimationCombo selfCombo = GetCombo(magic);
        if (selfCombo != null)
            SetupDurationHelper.SetupDurationIfHitTimeNot0(
                magic, entity.Identity.Id, entity, selfCombo);
        PhysicalSkillPresentation.TryPlay(entity, magic, ResolveEvents(entity));
        SkillVisualEffectDispatch.PlayIfKnown(entity, magic, selfCombo);
    }

    static void PlaySkillEffect(Entity entity, MagicSkillUseDto magic)
    {
        bool windstrike = magic != null && SkillPresentationCatalog.IsWindStrike(magic.SkillId);
        if (entity == null || magic == null)
        {
            if (windstrike)
            {
                Debug.Log(
                    "[WINDSTRIKE] PlaySkillEffect abort" +
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
        SkillVisualEffectDispatch.PlayIfKnown(entity, magic);
        if (windstrike)
            Debug.Log("[WINDSTRIKE] PlaySkillEffect returned");
    }

    static bool TryPlayNoneAnimWorldPawnEffect(Entity entity, MagicSkillUseDto magic)
    {
        bool windstrikeGate = magic != null && SkillPresentationCatalog.IsWindStrike(magic.SkillId);
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
            magic.SkillLvl,
            target != null ? target.transform : entity.AttackTarget);
        entity.SetupTotalCastDuration(
            magic.HitTime,
            flightTimeMs,
            Array.Empty<float>(),
            0f,
            magic.TargetId,
            SetupDurationHelper.CastStyleOf(magic),
            SetupDurationHelper.MixComboOf(magic),
            magic.SkillId);

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
            ControllerPacketServices.Animations.PlayCreatureAnimation(
                entity.Identity.Id, sequence);
        }

        return true;
    }
}
