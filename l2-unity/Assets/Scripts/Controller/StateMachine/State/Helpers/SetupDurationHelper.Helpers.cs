using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SetupDurationHelper
{
    public static void PlayPotionEffect(Entity entity, MagicSkillUseDto useSkill)
    {
        if (entity == null || useSkill == null)
            return;

        SkillVisualEffectDispatch.PlayIfKnown(entity, useSkill);
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

    /// <summary>
    /// One InitSkillProcess setup for every skill with a hit time: self casts and targeted
    /// PHYS Mix combos. Clip lengths are override slots or SpAtk combat takes. Flight comes
    /// from the skill visual catalog (0 when the cast target is the caster).
    /// </summary>
    public static void SetupDurationIfHitTimeNot0(MagicSkillUseDto useSkill, int objectId, Entity entity, AnimationCombo selfCombo)
    {
        if (useSkill == null || entity == null || selfCombo == null || useSkill.HitTime <= 0)
            return;

        string[] orderedSelfCycle = BuildOrderedCycleForOverrideTiming(selfCombo.GetAnimCycle());
        float[] durations = System.Array.Empty<float>();
        float shotEventTime = 0f;
        if (ControllerPacketServices.Animations != null && orderedSelfCycle.Length > 0)
        {
            durations = ControllerPacketServices.Animations.GetOverrideClipsDurations(objectId, orderedSelfCycle);
            shotEventTime = ResolveShotEventTime(objectId, orderedSelfCycle);
        }

        bool selfCast = entity.Identity == null || useSkill.TargetId == entity.Identity.Id;
        Transform flightTarget = selfCast ? entity.transform : null;
        float flightMs = ResolveMagicFlightTimeMs(entity, useSkill.SkillId, useSkill.SkillLvl, flightTarget);
        entity.SetupTotalCastDuration(
            useSkill.HitTime,
            flightMs,
            durations,
            shotEventTime,
            useSkill.TargetId,
            CastStyleOf(useSkill),
            MixComboOf(useSkill),
            useSkill.SkillId);
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

    public static float ResolveMagicFlightTimeMs(Entity entity, int skillId, int skillLevel, Transform target)
    {
        // Self skills have nothing flying. The authored usk flying time is for a bolt
        // between two pawns, not a distance guess and not a GlobalEffect flag.
        if (entity != null && ReferenceEquals(target, entity.transform))
            return 0f;

        return SkillVisualRuntime.FlightSeconds(skillId, skillLevel) * 1000f;
    }
}
