using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public partial class AnimationManager
{
    private static bool IsMagicCastTrigger(string animationName)
    {
        return animationName == CAST_TRIGGER_MID ||
               animationName == CAST_TRIGGER_END ||
               animationName == CAST_TRIGGER_SHOT ||
               animationName == CAST_TRIGGER_END_2P ||
               animationName == CAST_TRIGGER_SHOT_2P;
    }

    private static bool IsMagicShotTrigger(string animationName)
    {
        return animationName == CAST_TRIGGER_SHOT ||
               animationName == CAST_TRIGGER_SHOT_2P ||
               animationName.StartsWith(CAST_TRIGGER_SHOT, StringComparison.Ordinal);
    }
}
