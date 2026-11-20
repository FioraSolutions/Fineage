using UnityEngine;

/// <summary>
/// Player_Basic <c>stand</c> has no graph exit: when the stand-up clip ends, CrossFade back to the
/// weapon wait pose (same hand-off as <see cref="PlayerStatePickup"/>). A fresh sit keeps the pose.
/// </summary>
public class PlayerStateStand : PlayerStateBase
{
    const float FinishNorm = 0.95f;
    bool _switched;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _switched = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_switched || stateInfo.normalizedTime < FinishNorm || animator.IsInTransition(layerIndex))
            return;

        _switched = true;
        int objectId = AnimatorUtils.GetObjectId(animator);
        IAnimationManager animations = AnimationPacketServices.Animations;
        if (animations == null || objectId == 0 || EntityActionVisual.IsSitPoseHeld(objectId))
            return;

        AnimationPacketServices.QueueOnMain(() =>
            animations.PlayAnimation(objectId, AnimationNames.WAIT.ToString(), true));
    }
}
