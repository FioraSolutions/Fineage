using System.Collections.Generic;
using UnityEngine;

public static partial class AnimationDataCache
{
    public static AnimationClip GetOverrideClip(Animator animator, string originalMotionName)
    {
        if (string.IsNullOrEmpty(originalMotionName) || animator == null) return null;

        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        string cacheKey = controller.name + "_" + originalMotionName;

        if (_clipCache.TryGetValue(cacheKey, out AnimationClip cachedClip))
        {
            return cachedClip;
        }

        AnimationClip finalClip = null;
        var overrideController = controller as AnimatorOverrideController;

        if (overrideController != null)
        {
            // Прямой доступ через индексатор (самый быстрый)
            finalClip = overrideController[originalMotionName];

            // Если через индексатор не вышло, ищем в списке переопределений
            if (finalClip == null)
            {
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
                overrideController.GetOverrides(overrides);
                foreach (var pair in overrides)
                {
                    if (pair.Key != null && pair.Key.name == originalMotionName)
                    {
                        finalClip = pair.Value != null ? pair.Value : pair.Key;
                        break;
                    }
                }
            }
        }
        else
        {
            
            foreach (var clip in controller.animationClips)
            {
                if (clip.name == originalMotionName)
                {
                    finalClip = clip;
                    break;
                }
            }
        }

        if (finalClip != null) _clipCache[cacheKey] = finalClip;
        return finalClip;
    }

    public static float GetOverrideLength(Animator animator, string originalMotionName)
    {
        AnimationClip clip = GetOverrideClip(animator, originalMotionName);
        return clip != null ? clip.length : 1.0f;
    }

    public static float GetEventTimeByName(Animator animator, string originalMotionName, string eventName)
    {
        AnimationClip clip = GetOverrideClip(animator, originalMotionName);
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
}
