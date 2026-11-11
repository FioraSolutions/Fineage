using UnityEngine;

public partial class PlayerStateJAtk : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int objectId = animator.GetInteger(AnimatorUtils.OBJECT_ID);
        if (!_rateFromSwingClip &&
            PlayerBasicAttackAnim.TryGetSwingClipLength(animator, layerIndex, out float swingLen, out _))
        {
            _clipLengthSec = swingLen;
            _linearPatkSpd = Animations.ApplyLinearMeleePAtkSpeed(
                objectId, parameterName, swingLen);
            _rateFromSwingClip = true;
        }
        if (_startTime >= 0f && _rateFromSwingClip)
            Animations.SetPAtkSpeed(objectId, _linearPatkSpd);

        if (!AnimatorUtils.IsLocalPlayerAnimator(animator))
            return;

        float timeOut = Time.time - _startTime;
        _lastAnimNormalized = stateInfo.normalizedTime % 1f;
        LogAnimMilestones(timeOut);
        LogDieTargetLatch(timeOut);

        if (timeOut >= _endTime)
        {
            SwitchToIdle(stateInfo);
        }

        int entityId = ResolvePlayerEntityId();
        if (entityId > 0 && SwordCollisionPacketServices.Collision != null)
        {
            SwordCollisionPacketServices.Collision.UpdateAttackProgress(entityId, TimeUtils.ConvertSecToMs(timeOut));
        }
    }

    private void LogDieTargetLatch(float elapsedSec)
    {
        if (_dieTargetLatched)
        {
            return;
        }

        if (!IsDieTarget())
        {
            return;
        }

        _dieTargetLatched = true;
        float elapsedMs = TimeUtils.ConvertSecToMs(elapsedSec);
        bool isAttack = DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.IsAttack;
        // Debug.Log(
            // $"{JATK_EARLY_LOG} DIE_TARGET_LATCH anim={parameterName} " +
            // $"wallElapsedMs={elapsedMs:F1}/{_fullCycleMs:F1} animNorm={_lastAnimNormalized:F3} " +
            // $"isAttack={isAttack} " +
            // $"note=SwitchToIdle_still_waits_for_wall_cycle_unless_external_CrossFade");
    }
}
