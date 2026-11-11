using UnityEngine;

public partial class MagicCastData
{
    public void Setup(
        float serverHitMs,
        float flyMs,
        float[] clipsDurations,
        float shotEventTime,
        int targetObjectId = 0,
        int castStyle = 1,
        bool mixCombo = false)
    {
        StartTime = Time.time;
        HitTime = serverHitMs / 1000f;
        FlightTime = flyMs / 1000f;
        TargetObjectId = targetObjectId;
        AnimationShotAtSeconds = 0f;
        SpawnShotVisualsImmediately = false;
        PhaseWallMidSeconds = 0f;
        PhaseWallEndSeconds = 0f;
        PhaseWallShotSeconds = 0f;
        _clipCount = 0;
        _fillerIndex = 0;
        _play0 = _play1 = _play2 = 0f;

        serverTimeToShoot = Mathf.Max(0.01f, HitTime - FlightTime);

        if (MagicCastWallPenaltyDisabledForTesting)
        {
            AnimatorWallPenaltySeconds = 0f;
            AdjustedShootWindowSeconds = Mathf.Max(
                0.01f,
                serverTimeToShoot - CastShootWindowFixedOffsetSeconds);
        }
        else
        {
            float hitClamped = Mathf.Clamp(
                HitTime,
                WallPenaltyClampHitMinSeconds,
                WallPenaltyClampHitMaxSeconds);
            float penaltyTLinear = Mathf.InverseLerp(
                WallPenaltyClampHitMinSeconds,
                WallPenaltyClampHitMaxSeconds,
                hitClamped);
            AnimatorWallPenaltySeconds = Mathf.Lerp(WallPenaltySecondsMin, WallPenaltySecondsMax, penaltyTLinear);

            AdjustedShootWindowSeconds = Mathf.Max(
                0.01f,
                serverTimeToShoot - AnimatorWallPenaltySeconds);
        }

        float durMid = GetDurationSafe(clipsDurations, 0);
        float durEnd = GetDurationSafe(clipsDurations, 1);
        float durShot = GetDurationSafe(clipsDurations, 2);
        DurMid = durMid;
        DurEnd = durEnd;
        DurShot = durShot;

        bool hasMidPhase = durMid > 0f;
        bool hasEndPhase = durEnd > 0f;
        bool hasShotPhase = durShot > 0f;
        bool isTwoClipCastPath = hasMidPhase && hasEndPhase && !hasShotPhase;

        float durShotToEvent = isTwoClipCastPath
            ? ((shotEventTime > 0f) ? shotEventTime : durEnd)
            : ((shotEventTime > 0f) ? shotEventTime : durShot);
        this.shotEventTime = durShotToEvent;

        if (isTwoClipCastPath)
        {
            // Tags J/K/L: CastEnd then MagicShot — filler is the first clip.
            ApplyClientCastSpeeds(2, durMid, durEnd, 0f, durShotToEvent, castStyle, mixCombo);
        }
        else if (hasMidPhase && hasEndPhase && hasShotPhase)
        {
            ApplyClientCastSpeeds(3, durMid, durEnd, durShot, durShotToEvent, castStyle, mixCombo);
        }
        else if (hasMidPhase)
        {
            ApplyClientCastSpeeds(1, durMid, 0f, 0f, durShotToEvent, castStyle, mixCombo);
        }
        else
        {
            Debug.LogWarning(
                "[CastSync] ANIM.setup no mid clip hit=" + HitTime.ToString("F3") +
                " durs=[" + durMid.ToString("F3") + "," + durEnd.ToString("F3") + "," + durShot.ToString("F3") + "]");
            float timelineDuration = Mathf.Max(0.01f, durMid + durEnd + durShotToEvent);
            float timingScale = timelineDuration / AdjustedShootWindowSeconds;
            // Fallback still allows stretch (InitSkillProcess); no freeze-hold.
            SpeedMid = timingScale;
            SpeedEnd = timingScale;
            SpeedShot = timingScale;
            float played = timelineDuration / Mathf.Max(timingScale, 0.0001f);
            PhaseWallMidSeconds = played;
            PhaseWallEndSeconds = played;
            PhaseWallShotSeconds = played;
            AnimationShotAtSeconds = played;
            _clipCount = 1;
            _fillerIndex = 0;
            _dur0 = durMid;
            _play0 = played;
        }

        PinAnimationShotToServerLeave();
    }
}
