using UnityEngine;

public partial class PlayerStateJatkBow : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isSwitchIdle = false;
        _startTime = Time.time;

        AnimationClip clip = AnimationDataCache.GetOverrideClip(animator, motionName);
        float clipLength = clip != null ? clip.length : AnimationDataCache.GetOverrideLength(animator, motionName);
        if (clipLength <= 0.01f)
            clipLength = Mathf.Max(stateInfo.length, 0.01f);

        int objectId = AnimatorUtils.GetObjectId(animator);
        World world = WorldApplyPacketServices.GameWorld;
        Entity entity = world != null ? world.GetEntityNoLockSync(objectId) : null;

        // Same linear attack-cycle pipeline as melee / SpAtkBow (500000/pAtkSpd, or sptimeatk).
        if (Animations != null)
            Animations.ApplyLinearMeleePAtkSpeed(objectId, string.IsNullOrEmpty(motionName) ? parameterName : motionName, clipLength);

        float cycleMs = AttackTimingHelper.ResolveAttackCycleMs(
            entity, string.IsNullOrEmpty(motionName) ? parameterName : motionName);
        _endTime = _startTime + TimeUtils.ConvertMsToSec(cycleMs);

        if (AnimatorUtils.IsLocalPlayerAnimator(animator) && DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.ClearEtcRelease();
            if (DomainPacketServices.LocalPlayer.GetGoEtcItem() == null)
                DomainPacketServices.LocalPlayer.EquipArrow(17);
        }

        if (AnimatorUtils.IsLocalPlayerAnimator(animator))
            StopAnimationTrigger(animator, parameterName);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_startTime < 0f)
            return;

        if (!AnimatorUtils.IsLocalPlayerAnimator(animator))
            return;

        if (Time.time >= _endTime)
            SwitchToIdle();
    }
}
