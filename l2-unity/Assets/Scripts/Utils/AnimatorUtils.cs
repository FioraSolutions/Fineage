using UnityEngine;

public class AnimatorUtils
{
    public static string OBJECT_ID = "objectId";

    public static int GetObjectId(Animator animator)
    {
        if (animator == null)
            return 0;
        return animator.GetInteger(OBJECT_ID);
    }

    /// <summary>
    /// True only for the local player's Animator. Remote UserEntity SMBs must not
    /// touch PlayerAnimationController / PlayerStateMachine.
    /// </summary>
    public static bool IsLocalPlayerAnimator(Animator animator)
    {
        if (animator == null || DomainPacketServices.LocalPlayer == null)
            return false;

        PlayerAnimationController playerAnimation = AnimationPacketServices.PlayerAnimation;
        if (playerAnimation != null)
        {
            Animator local = playerAnimation.GetAnimator();
            if (local != null)
                return local == animator;
        }

        if (DomainPacketServices.LocalPlayer.Identity == null)
            return false;
        return animator.GetInteger(OBJECT_ID) == DomainPacketServices.LocalPlayer.Identity.Id;
    }

}
