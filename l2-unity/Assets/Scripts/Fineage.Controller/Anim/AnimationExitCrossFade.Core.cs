using System.Collections.Generic;
using UnityEngine;

public static partial class AnimationExitCrossFade
{
    static void CollectExitOverrides(Animation[] source, List<Animation> dest)
    {
        if (source == null)
        {
            return;
        }

        for (int i = 0; i < source.Length; i++)
        {
            Animation anim = source[i];
            if (anim != null && anim.ExitCrossFadeDuration.HasValue)
            {
                dest.Add(anim);
            }
        }
    }
}
