using UnityEngine;

public class NpcStateWait : NpcStateBase
{
    public int playSpWaitChancePercent = 3;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool("spwait", false);
        SetBool("wait", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RestartNonLoopingClip(animator, stateInfo, layerIndex);

        NpcIdleScheduler sched = NpcIdleScheduler.Ensure(_entity);
        if (sched != null && sched.TryTriggerSpecialIdle())
        {
            if (AnimatorBoolSet.HasBool(animator, "spwait"))
            {
                SetBool("spwait", true);
                sched.OnSpecialIdleTriggered();
                return;
            }
        }

        if (sched == null
            && playSpWaitChancePercent > 0
            && stateInfo.normalizedTime >= 0.92f
            && RandomUtils.ShouldEventHappen(playSpWaitChancePercent))
        {
            SetBool("spwait", true);
        }

        if (audioHandler != null
            && RandomUtils.ShouldEventHappen(audioHandler.WaitSoundChance))
            audioHandler.PlaySound(CharacterSoundEvent.Admiration);
    }

    static void RestartNonLoopingClip(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.loop)
            return;

        float cycle = stateInfo.normalizedTime % 1f;
        if (cycle < 0.98f && stateInfo.normalizedTime < 0.98f)
            return;

        animator.Play(stateInfo.fullPathHash, layerIndex, 0f);
    }
}