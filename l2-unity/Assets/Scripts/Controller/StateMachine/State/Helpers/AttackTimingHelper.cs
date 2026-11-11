using UnityEngine;

public static partial class AttackTimingHelper
{
    // Player 1HS (server): HitTime/full = timeAtk, onHitTimer sAtk = timeAtk / 2 → fraction 0.5
    private const float SIMPLE_MELEE_HIT_OVER_FULL = 0.5f;
    // Interlude BowAttackProcess live capture:
    // GetAtk01AnimRate=0.8333=25/30, PlayAnim rate=GetAtk01AnimRate*AttackSpeedMultiplier,
    // TweenTime=0.2/PlayRate. Player bow source sequences report rate=30.
    private const float INTERLUDE_BOW_SOURCE_FPS = 30f;
    private const float INTERLUDE_BOW_BASE_FPS = 25f;
    private const float INTERLUDE_BOW_TWEEN_BASE_SECONDS = 0.2f;

    /// <summary>
    /// Disabled: procedural spine/arm tilt toward the target. Keep for later.
    /// </summary>

    /// <summary>
    /// Full attack cycle = Formulas.calculateTimeBetweenAttacks (500000 / pAtkSpd) ≈ HitTime.
    /// </summary>

    /// <summary>
    /// PlayerEntity uses UserInfo BasePAtkSpeed; UserEntity CharInfo only fills PAtkSpd.
    /// </summary>

    /// <summary>
    /// L2J timeAtk ms. Bow subtracts arrow flight from the draw window when distance is known.
    /// </summary>

    /// <summary>
    /// Bow release wall time. Server Hit is the full 500000/PAtkSpd delay.
    /// The arrow still flies with ProjectileFlightTimeCalculator, so the clip
    /// notify must happen at Hit minus that flight. No 1000 ms draw cap.
    /// </summary>

    /// <summary>
    /// Reproduces Interlude APawn::BowAttackProcess animation timing.
    /// Distance and projectile flight do not affect the animation or AttackShot notify.
    /// Unity clips preserve original frame numbers on their imported sample-rate timeline,
    /// therefore the original 30 fps PlayAnim rate is converted to the clip sample rate.
    /// </summary>

    /// <summary>
    /// Animator <c>patkspd</c>: play the whole clip over server timeAtk (not timeAtk/2).
    /// </summary>

    /// <summary>
    /// Fraction of full cycle when server fires onHitTimer.
    /// Player simple melee: doAttackHitSimple(..., timeAtk / 2) → 0.5.
    /// </summary>

}
