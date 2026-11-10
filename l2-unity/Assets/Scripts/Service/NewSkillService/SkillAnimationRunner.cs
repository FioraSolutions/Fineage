using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SkillAnimationRunner
{
    private const string CAST_TRIGGER_MID_OVERRIDE = "CastMid";
    private const string CAST_TRIGGER_END_OVERRIDE = "CastEnd";
    private const string CAST_TRIGGER_SHOT_OVERRIDE = "MagicShot";
    private const string CAST_TRIGGER_END_2P = "CastEnd2P";
    private const string CAST_TRIGGER_SHOT_2P = "MagicShot2P";
    private const string NONE_ANIMATION_NAME = "none";

    /// <summary>Wolf Collar 2046: log why the cast clips do not start.</summary>
    public static bool Trace;

   public async Task StartRun(string[] cycle , int objectId , IAnimationManager animationManager , Action onComplete = null)
    {
        if (cycle == null)
        {
            // Debug.LogWarning($"[SkillRun] cycle is null objectId={objectId}");
            onComplete?.Invoke();
            return;
        }

        // Debug.Log($"[SkillRun] START objectId={objectId} cycleLength={cycle.Length}");

        for (int i = 0; i < cycle.Length; i++)
        {
            string animName = cycle[i];
            if (ShouldSkipAnimation(animName))
            {
                // Debug.Log($"[SkillRun] SKIP objectId={objectId} idx={i} rawAnim='{animName}' reason=none_or_empty");
                continue;
            }

            string sanitizedAnimName = animName.Trim();
            string triggerToPlay = sanitizedAnimName + "_";

            bool phaseOk = await animationManager.AsyncPlayAnimationTrigger(objectId, triggerToPlay);
            if (!phaseOk)
            {
                // Debug.Log(
                    // $"[SkillRun] ABORT superseded objectId={objectId} idx={i} trigger='{triggerToPlay}' " +
                    // $"(no onComplete / WAIT_RETURN)");
                return;
            }
        }

        onComplete?.Invoke();
    }

    public async Task StartRunOverride(string[] cycle, int objectId, IAnimationManager animationManager, Action onComplete = null)
    {
        if (cycle == null)
        {
            // Debug.LogWarning($"[SkillRunOverride] cycle is null objectId={objectId}");
            onComplete?.Invoke();
            return;
        }

        float chainStartTime = Time.time;
        var validAnimations = new List<string>();
        for (int i = 0; i < cycle.Length; i++)
        {
            string animName = cycle[i];
            bool shouldSkip = ShouldSkipAnimation(animName);
            if (!shouldSkip)
            {
                validAnimations.Add(animName.Trim());
            }
        }

        string[] triggerPlan = BuildTriggerPlan(validAnimations.Count);
        Debug.Log(
            "[CastSync] ANIM.runStart objectId=" + objectId +
            " valid=" + string.Join(",", validAnimations) +
            " triggers=" + string.Join(",", triggerPlan));
        if (Trace)
        {
            Debug.Log(
                "[SUMMON_ANIM] plan objectId=" + objectId +
                " valid=" + string.Join(",", validAnimations) +
                " triggers=" + string.Join(",", triggerPlan) +
                " animations=" + (animationManager != null));
        }

        if (animationManager == null)
        {
            if (Trace)
            {
                Debug.Log("[SUMMON_ANIM] abort animations=null objectId=" + objectId);
                Trace = false;
            }
            return;
        }

        for (int logicalIndex = 0; logicalIndex < validAnimations.Count; logicalIndex++)
        {
            string overrideAnimName = validAnimations[logicalIndex];
            string triggerName = triggerPlan[logicalIndex];

            if (string.IsNullOrEmpty(triggerName))
            {
                // Debug.LogWarning($"[SkillRunOverride] SKIP objectId={objectId} logicalIdx={logicalIndex} override='{overrideAnimName}' reason=empty_trigger_name");
                continue;
            }

            // Debug.Log(
                // $"[SkillRunOverride] BEFORE await logicalIdx={logicalIndex} trigger='{triggerName}' override='{overrideAnimName}' " +
                // $"sinceChainStart={Time.time - chainStartTime:F3}s objectId={objectId}");
            Debug.Log(
                "[CastSync] ANIM.awaitPhase t=" + (Time.time - chainStartTime).ToString("F3") +
                " idx=" + logicalIndex +
                " trigger=" + triggerName +
                " override=" + overrideAnimName);
            bool phaseOk = await animationManager.AsyncPlayAnimationRaceOverrides(objectId, triggerName, overrideAnimName);
            Debug.Log(
                "[CastSync] ANIM.phaseDone t=" + (Time.time - chainStartTime).ToString("F3") +
                " trigger=" + triggerName +
                " ok=" + phaseOk);
            if (Trace)
            {
                Debug.Log(
                    "[SUMMON_ANIM] phase objectId=" + objectId +
                    " trigger=" + triggerName +
                    " override=" + overrideAnimName +
                    " ok=" + phaseOk);
            }
            if (!phaseOk)
            {
                if (Trace)
                    Trace = false;
                return;
            }
        }

        if (Trace)
        {
            Debug.Log("[SUMMON_ANIM] chain complete objectId=" + objectId);
            Trace = false;
        }
        onComplete?.Invoke();
    }

    private static bool ShouldSkipAnimation(string animName)
    {
        if (string.IsNullOrWhiteSpace(animName))
        {
            return true;
        }

        return string.Equals(animName.Trim(), NONE_ANIMATION_NAME, StringComparison.OrdinalIgnoreCase);
    }

    private static string[] BuildTriggerPlan(int validAnimationsCount)
    {
        if (validAnimationsCount <= 0)
        {
            return Array.Empty<string>();
        }

        if (validAnimationsCount == 1)
        {
            // Single animation fallback: use CastEnd because it is the safest cast completion state.
            return new[] { CAST_TRIGGER_END_OVERRIDE };
        }

        if (validAnimationsCount == 2)
        {
            // Two-clip cast uses dedicated animator states with their own transitions.
            return new[] { CAST_TRIGGER_END_2P, CAST_TRIGGER_SHOT_2P };
        }

        // Default full chain for 3+ clips.
        string[] plan = new string[validAnimationsCount];
        plan[0] = CAST_TRIGGER_MID_OVERRIDE;
        plan[1] = CAST_TRIGGER_END_OVERRIDE;
        plan[2] = CAST_TRIGGER_SHOT_OVERRIDE;

        for (int i = 3; i < validAnimationsCount; i++)
        {
            plan[i] = CAST_TRIGGER_SHOT_OVERRIDE;
        }

        return plan;
    }
}
