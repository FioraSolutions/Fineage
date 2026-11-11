using UnityEngine;

public partial class PlayerStateSpAtkBow : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isSwitchIdle = false;
        _startTime = Time.time;

        float clipLength = AnimationDataCache.GetOverrideLength(animator, motionName);
        float eventTimeInClip = AnimationDataCache.GetEventTimeByName(animator, motionName, eventShootName);
        if (clipLength <= 0.01f)
            clipLength = Mathf.Max(stateInfo.length, 0.01f);
        if (eventTimeInClip <= 0.01f)
            eventTimeInClip = clipLength * 0.637f;

        int objectId = AnimatorUtils.GetObjectId(animator);
        int serverTimeMs = animator.GetInteger("sptimeatk");
        World world = WorldApplyPacketServices.GameWorld;
        Entity entity = world != null ? world.GetEntityNoLockSync(objectId) : null;
        float cycleMs = serverTimeMs > 0
            ? serverTimeMs
            : AttackTimingHelper.ResolveAttackCycleMs(entity, parameterName);
        _endTime = Mathf.Max(0.01f, cycleMs / 1000f);
        _linearSpeed = AttackTimingHelper.ComputeLinearPAtkSpeed(clipLength, cycleMs);
        Animations.SetPAtkSpeed(objectId, _linearSpeed);
        // Debug.Log(
            // $"[PATKSPD] SpAtkBow objectId={objectId} anim={parameterName} " +
            // $"sptimeatk={serverTimeMs} cycleMs={cycleMs:F1} clipSec={clipLength:F3} patkspd={_linearSpeed:F3}");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int objectId = AnimatorUtils.GetObjectId(animator);
        Animations.SetPAtkSpeed(objectId, _linearSpeed);

        if (!AnimatorUtils.IsLocalPlayerAnimator(animator))
            return;

        float timeOut = Time.time - _startTime;
        if (timeOut >= _endTime)
            SwitchToIdle(animator);
    }
}
