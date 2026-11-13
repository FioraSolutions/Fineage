using System.Collections.Generic;
using UnityEngine;

public class NpcStateSpWait : NpcStateBase {
    bool started = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        audioHandler.PlaySound(CharacterSoundEvent.Yawn);

        SetBool("spwait", false);
        started = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // 1.0 % 1 == 0, so a non-looping special-wait that lands on the last frame never returns to wait.
        if(started && stateInfo.normalizedTime >= 0.90f) {
            started = false;
            SetBool("wait", true);       
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
