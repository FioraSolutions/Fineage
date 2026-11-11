using UnityEngine;

public partial class PlayerStateSpAtk : StateMachineBehaviour
{
    public static bool TryEstimateWallDuration(
        Animator animator,
        string spatkAnimName,
        int serverHitTimeMs,
        out float wallDurationSeconds,
        out string debugMotionName)
    {
        wallDurationSeconds = -1f;
        debugMotionName = null;
        if (animator == null || serverHitTimeMs <= 0)
        {
            return false;
        }

        if (!TryResolveBehaviour(animator, spatkAnimName, out PlayerStateSpAtk behaviour) || behaviour == null)
        {
            return false;
        }

        debugMotionName = behaviour.motionName;
        float clipLength = AnimationDataCache.GetOverrideLength(animator, behaviour.motionName);
        float startHit = AnimationDataCache.GetEventTimeByName(animator, behaviour.motionName, behaviour.eventStartHitName);
        float endHit = AnimationDataCache.GetEventTimeByName(animator, behaviour.motionName, behaviour.eventEndHitName);
        float hitWindowCenter = (startHit + endHit) * 0.5f;
        if (hitWindowCenter <= 0.01f)
        {
            return false;
        }

        float serverHitTime = (serverHitTimeMs / 1000f) - HitTimeCompensationSeconds;
        if (serverHitTime < MinServerHitSeconds)
        {
            serverHitTime = MinServerHitSeconds;
        }

        float linearSpeed = hitWindowCenter / serverHitTime;
        wallDurationSeconds = (SwitchToIdleNormalizedTime * clipLength) / Mathf.Max(0.01f, linearSpeed);
        return wallDurationSeconds > 0f;
    }
}
