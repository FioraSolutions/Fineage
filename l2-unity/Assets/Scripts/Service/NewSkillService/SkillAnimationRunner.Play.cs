using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class SkillAnimationRunner
{
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
