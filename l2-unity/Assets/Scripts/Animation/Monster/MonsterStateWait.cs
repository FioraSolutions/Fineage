using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateSpWait : StateMachineBehaviour
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    public string parameterName;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Animations.StopMonsterCurrentAnimation(animator.GetInteger(AnimatorUtils.OBJECT_ID), parameterName);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
