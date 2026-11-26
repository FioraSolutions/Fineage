using UnityEngine;

public class NpcStateSpWait : NpcStateBase
{
    bool started;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        audioHandler?.PlaySound(CharacterSoundEvent.Yawn);
        SetBool("spwait", false);
        started = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInfo.loop && stateInfo.normalizedTime >= 0.98f)
            animator.Play(stateInfo.fullPathHash, layerIndex, 0f);

        if (started && stateInfo.normalizedTime >= 0.92f)
        {
            started = false;
            SetBool("wait", true);
            NpcIdleScheduler.Ensure(_entity)?.Reschedule();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("spwait", false);
    }
}