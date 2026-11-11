using UnityEngine;

public partial class PlayerOverriddenMagicAtk : StateMachineBehaviour
{
    private static bool ShouldUseForceSync(MagicCastData castData)
    {
        return castData != null && EnableForceSync && IsHitTimeInsideForceSyncBand(castData);
    }

    private static bool IsHitTimeInsideForceSyncBand(MagicCastData castData)
    {
        return castData != null && castData.HitTime >= ForceSyncMinServerHitTimeSeconds;
    }

    private void StopAnimationTrigger(Animator animator, string parameterName)
    {
        if (!string.IsNullOrEmpty(parameterName) && animator.GetBool(parameterName))
        {
            Animations.StopCurrentAnimation(animator.GetInteger(AnimatorUtils.OBJECT_ID), parameterName, "player");
        }
    }
}
