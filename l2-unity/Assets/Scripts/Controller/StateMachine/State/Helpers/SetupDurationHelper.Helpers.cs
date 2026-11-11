using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SetupDurationHelper
{
    public static void PlayPotionEffect(Entity entity, MagicSkillUseDto useSkill)
    {
        EffectManager effects = ControllerPacketServices.Effects;
        if (entity == null || useSkill == null || effects == null)
            return;

        effects.PlayEffect(useSkill.SkillId, entity.transform, entity.GetMagicCastData());
    }

    public static void FinishPotionUse(PlayerStateMachine stateMachine, Entity entity, MagicSkillUseDto useSkill)
    {
        PlayPotionEffect(entity, useSkill);
        if (stateMachine == null)
            return;

        bool inSkillState = stateMachine.State == PlayerState.MAGIC_SKILLS
            || stateMachine.State == PlayerState.PHYSICAL_SKILLS;
        if (!inSkillState)
            return;

        stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
        if (IsPickupAnimationPlaying(stateMachine.GetObjectId()))
            return;

        stateMachine.NotifyEvent(Event.WAIT_RETURN);
    }

    public static void SetupDurationIfHitTimeNot0(MagicSkillUseDto useSkill, int objectId, Entity entity, AnimationCombo selfCombo)
    {

        if (useSkill.HitTime > 0)
        {
            string[] orderedSelfCycle = BuildOrderedCycleForOverrideTiming(selfCombo.GetAnimCycle());
            float[] durations_self = ControllerPacketServices.Animations.GetOverrideClipsDurations(objectId, orderedSelfCycle);
            float shotEventTime_self = ResolveShotEventTime(objectId, orderedSelfCycle);
            entity.SetupTotalCastDuration(useSkill.HitTime, 0f, durations_self, shotEventTime_self, useSkill.TargetId, CastStyleOf(useSkill), MixComboOf(useSkill));
        }
    }

    public static string[] BuildOrderedCycleForOverrideTiming(string[] rawCycle)
    {
        if (rawCycle == null || rawCycle.Length == 0)
        {
            return Array.Empty<string>();
        }

        List<string> valid = new List<string>();
        for (int i = 0; i < rawCycle.Length; i++)
        {
            string clipName = rawCycle[i];
            if (string.IsNullOrWhiteSpace(clipName))
            {
                continue;
            }

            if (string.Equals(clipName.Trim(), "none", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            valid.Add(clipName.Trim());
        }

        // Keep timing order consistent with SkillAnimationRunner trigger plan.
        if (valid.Count == 2)
        {
            return new[] { valid[0], valid[1] };
        }

        return valid.ToArray();
    }

    public static float ResolveShotEventTime(int objectId, string[] orderedCycle)
    {
        if (orderedCycle == null || orderedCycle.Length == 0)
        {
            return 0f;
        }

        // For two-clip execution (CastEnd -> MagicShot), shoot event belongs to the second clip.
        if (orderedCycle.Length == 2)
        {
            return ControllerPacketServices.Animations.GetOverrideEventTimeByName(
                objectId,
                new[] { orderedCycle[1] },
                "OnAnimationShoot");
        }

        return ControllerPacketServices.Animations.GetOverrideEventTimeByName(objectId, orderedCycle, "OnAnimationShoot");
    }

    public static float ResolveMagicFlightTimeMs(Entity entity, int skillId, Transform target)
    {
        const float fallbackFlightMs = 1000f;

        EffectManager effects = ControllerPacketServices.Effects;
        if (effects != null && effects.ShouldIgnoreFlightTimeForCast(skillId))
        {
            return 0f;
        }

        if (entity == null || target == null)
        {
            return fallbackFlightMs;
        }

        Vector3 startPos = entity.GetPositionRightHand();
        Vector3 aimPos = VectorWorldUtils.GetCollision(startPos, target);
        float distance = Vector3.Distance(startPos, aimPos);
        if (distance <= 0f)
        {
            return fallbackFlightMs;
        }

        float flightSeconds = ProjectileFlightTimeCalculator.CalculateL2SkillFlightTimeSeconds(distance);
        return flightSeconds * 1000f;
    }
}
