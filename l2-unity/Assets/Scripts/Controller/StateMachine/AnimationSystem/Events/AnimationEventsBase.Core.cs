using System;
using UnityEngine;

public abstract partial class AnimationEventsBase : MonoBehaviour
{
    public void OnAnimationComplete(string animationName)
    {
        animationName = NormalizeAnimationEventName(animationName);
        string finishedName = SkillAnimationDatabase.ResolveOverrideCompletionEventName(animationName);

        if (Time.frameCount == _lastDuplicateCompleteFrame &&
            string.Equals(_lastDuplicateCompleteAnim, finishedName, StringComparison.Ordinal))
        {
            return;
        }

        _lastDuplicateCompleteFrame = Time.frameCount;
        _lastDuplicateCompleteAnim = finishedName;

        OnAnimationFinished?.Invoke(finishedName);
    }

    public virtual void OnAnimationShoot(string animationName)
    {
        animationName = NormalizeAnimationEventName(animationName);
        OnAnimationStartShoot?.Invoke(animationName);
    }

    private static string NormalizeAnimationEventName(string animationName)
    {
        if (string.IsNullOrEmpty(animationName))
        {
            return animationName;
        }

        // MagicNoTarget clips are played through the shared MagicShot animator state.
        // Some imported clips also carry the truncated event payload "MagicNoTarge".
        if (animationName == "MagicNoTarget" ||
            animationName == "MagicNoTarge" ||
            animationName == "MagicNotarget")
        {
            return "MagicShot";
        }

        return animationName;
    }
}
