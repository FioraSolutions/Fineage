using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class SkillAnimationRunner
{
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
}
