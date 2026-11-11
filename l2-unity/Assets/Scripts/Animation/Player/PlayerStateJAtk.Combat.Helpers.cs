using UnityEngine;
using System;

public partial class PlayerStateJAtk : StateMachineBehaviour
{
    private bool IsDieTarget()
    {
        World world = WorldApplyPacketServices.GameWorld;
        if (DomainPacketServices.LocalPlayer == null || world == null)
        {
            return false;
        }

        int targetId = DomainPacketServices.LocalPlayer.TargetId;
        if (targetId == 0)
        {
            return false;
        }

        // Despawned after DieDto → null; treat as dead so we still leave jatk.
        Entity entity = world.GetEntityNoLockSync(targetId);
        return entity == null || entity.IsDead();
    }

    private void StopAnimationTrigger(Animator animator, string animParameterName)
    {
        // CrossFade combat path: jatk* is a state name, not a Bool param on Player_Basic.
        if (!AnimatorBoolSet.HasBool(animator, animParameterName))
            return;
        if (animator.GetBool(animParameterName))
        {
            Animations.StopCurrentAnimation(
                animator.GetInteger(AnimatorUtils.OBJECT_ID),
                animParameterName,
                "player");
        }
    }

    private static bool IsDual(string animName) =>
        !string.IsNullOrEmpty(animName) &&
        animName.IndexOf("dual", System.StringComparison.OrdinalIgnoreCase) >= 0;
}
