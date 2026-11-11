using UnityEngine;

public partial class PlayerStateJatkBow : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isSwitchIdle = false;
        _startTime = Time.time;

        AnimationClip clip = AnimationDataCache.GetOverrideClip(animator, motionName);
        float clipLength = clip != null ? clip.length : AnimationDataCache.GetOverrideLength(animator, motionName);
        float eventTimeInClip = AnimationDataCache.GetEventTimeByName(animator, motionName, eventShootName);
        if (clipLength <= 0.01f)
            clipLength = Mathf.Max(stateInfo.length, 0.01f);
        if (eventTimeInClip <= 0.01f)
            eventTimeInClip = clipLength * 0.637f;
        if (eventTimeInClip > clipLength)
            eventTimeInClip = clipLength;

        int objectId = AnimatorUtils.GetObjectId(animator);
        World world = WorldApplyPacketServices.GameWorld;
        Entity entity = world != null ? world.GetEntityNoLockSync(objectId) : null;
        float unityClipFps = clip != null ? clip.frameRate : 24f;
        AttackTimingHelper.ResolveClientBowAnimation(
            entity,
            unityClipFps,
            out float attackMultiplier,
            out float clientAnimRate,
            out float clientPlayRate,
            out _tweenSeconds,
            out _motionSpeed);

        _releaseAt = _tweenSeconds + eventTimeInClip / _motionSpeed;
        _endTime = _tweenSeconds + clipLength / _motionSpeed;
        if (Animations != null)
            Animations.SetPAtkSpeed(objectId, 0f);

        // ReplayAttackTrace.Write(
            // "BOW_CLIP_ENTER",
            // entity,
            // entity != null ? entity.GetTargetEntity() : null,
            // "clipLength=" + clipLength.ToString("F3") +
            // " shootAt=" + eventTimeInClip.ToString("F3") +
            // " sourceFps=30.000" +
            // " unityFps=" + unityClipFps.ToString("F3") +
            // " animRate=" + clientAnimRate.ToString("F6") +
            // " atkMul=" + attackMultiplier.ToString("F6") +
            // " playRate=" + clientPlayRate.ToString("F6") +
            // " tween=" + _tweenSeconds.ToString("F6") +
            // " R=" + _releaseAt.ToString("F3") +
            // " motionSpeed=" + _motionSpeed.ToString("F6") +
            // " endAt=" + _endTime.ToString("F3") +
            // " parameter=" + parameterName);

        if (AnimatorUtils.IsLocalPlayerAnimator(animator))
            StopAnimationTrigger(animator, parameterName);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_startTime < 0f || Animations == null)
            return;

        int objectId = AnimatorUtils.GetObjectId(animator);
        float elapsed = Time.time - _startTime;
        // UE2 starts the sequence at a negative frame during TweenTime. Holding
        // Unity on the first sample for that wall time reproduces the notify timing.
        float speed = elapsed < _tweenSeconds ? 0f : _motionSpeed;
        Animations.SetPAtkSpeed(objectId, speed);

        if (!AnimatorUtils.IsLocalPlayerAnimator(animator))
            return;

        if (elapsed >= _endTime)
            SwitchToIdle();
    }
}
