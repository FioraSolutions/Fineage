using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class SkillAnimationRunner
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



}
