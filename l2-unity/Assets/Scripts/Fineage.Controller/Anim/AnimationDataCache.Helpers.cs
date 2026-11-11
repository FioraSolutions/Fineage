using UnityEngine;
using System.Collections.Generic;

public static partial class AnimationDataCache
{
    public static AnimationClip GetActiveClip(Animator animator, int layerIndex)
    {
        if (animator == null) return null;

        AnimatorClipInfo[] clipInfo;

        // Если аниматор в процессе перехода, нам нужен клип, К КОТОРОМУ мы идем
        if (animator.IsInTransition(layerIndex))
        {
            clipInfo = animator.GetNextAnimatorClipInfo(layerIndex);
        }
        else
        {
            clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        }

        if (clipInfo.Length > 0)
        {
            // Возвращает именно тот клип, который подставлен в OverrideController
            return clipInfo[0].clip;
        }

        return null;
    }

    public static float GetEventTimeByName(Animator animator, AnimationClip clip , string eventName)
    {
        if (clip == null) return 0f;

        foreach (var animEvent in clip.events)
        {
            if (animEvent.functionName == eventName)
            {
                return animEvent.time;
            }
        }

        // Debug.LogWarning($"[AnimCache] Event '{eventName}' не найден в клипе {clip.name}");
        return 0f;
    }

    public static void ClearCache() => _clipCache.Clear();
}
