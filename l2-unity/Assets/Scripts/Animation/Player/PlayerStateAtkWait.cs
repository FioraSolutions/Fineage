using UnityEngine;

public class PlayerStateAtkWait : PlayerStateBase
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    public string parameterName;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Animations.StopCurrentAnimation(animator.GetInteger(AnimatorUtils.OBJECT_ID), parameterName);
    }
}
