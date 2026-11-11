using UnityEngine;

public static partial class AttackTimingHelper
{
    public static void ResolveBowRelease(
        Entity entity,
        out float hitSec,
        out float flightSec,
        out float releaseSec,
        out float distance)
    {
        hitSec = CalcBaseParam.CalculateTimeL2j(ResolvePAtkSpd(entity)) / 1000f;
        distance = 0f;
        flightSec = 0.01f;
        Transform target = entity != null ? entity.Target : null;
        if (entity != null && target != null)
        {
            Entity targetEntity = target.GetComponent<Entity>();
            if (targetEntity != null && targetEntity.Appearance != null)
            {
                Vector3 start = entity.GetPositionRightHand();
                Vector3 aim = VectorWorldUtils.GetCollision(start, target);
                distance = Vector3.Distance(start, aim);
                flightSec = ProjectileFlightTimeCalculator.CalculateL2ArrowFlightTimeSeconds(distance);
            }
        }

        releaseSec = Mathf.Max(0.05f, hitSec - flightSec);
    }

    public static void ResolveClientBowAnimation(
        Entity entity,
        float unityClipFps,
        out float attackSpeedMultiplier,
        out float clientAnimRate,
        out float clientPlayRate,
        out float tweenSeconds,
        out float unityMotionSpeed)
    {
        attackSpeedMultiplier = 1f;
        if (entity != null && entity.Stats != null &&
            entity.Stats.PAtkSpd > 0 && entity.Stats.PAtkRealSpeed > 0.0001f)
        {
            attackSpeedMultiplier = entity.Stats.PAtkRealSpeed / entity.Stats.PAtkSpd;
        }
        if (attackSpeedMultiplier <= 0.0001f || attackSpeedMultiplier > 10f)
            attackSpeedMultiplier = 1f;

        clientAnimRate = INTERLUDE_BOW_BASE_FPS / INTERLUDE_BOW_SOURCE_FPS;
        clientPlayRate = clientAnimRate * attackSpeedMultiplier;
        if (clientPlayRate <= 0.0001f)
            clientPlayRate = clientAnimRate;

        tweenSeconds = INTERLUDE_BOW_TWEEN_BASE_SECONDS / clientPlayRate;
        float clipFps = unityClipFps > 0.0001f ? unityClipFps : 24f;
        unityMotionSpeed = clientPlayRate * INTERLUDE_BOW_SOURCE_FPS / clipFps;
    }

    public static float ComputeLinearPAtkSpeed(float clipLengthSec, float cycleMs)
    {
        return clipLengthSec * 1000f / Mathf.Max(1f, cycleMs);
    }

    public static float ResolveHitFractionByWeapon(PlayerEntity player)
    {
        string weaponAnim = player.GetCurrentAnimName();
        if (string.IsNullOrEmpty(weaponAnim)) return SIMPLE_MELEE_HIT_OVER_FULL;

        string lower = weaponAnim.ToLowerInvariant();
        if (lower.Contains("bow")) return 0.82f;

        return SIMPLE_MELEE_HIT_OVER_FULL;
    }

    public static float ResolveServerLikeHitMs(PlayerEntity player)
    {
        return ResolveServerLikeAttackDurationMs(player) * ResolveHitFractionByWeapon(player);
    }
}
