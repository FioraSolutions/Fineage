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

    /// <summary>
    /// Align <see cref="AnimationShotAtSeconds"/> to <see cref="serverTimeToShoot"/>.
    /// Late → speed up the whole chain. Early → stretch the filler clip (InitSkillProcess).
    /// Never freeze the animator.
    /// </summary>
    private void PinAnimationShotToServerLeave()
    {
        float target = serverTimeToShoot;
        float current = AnimationShotAtSeconds;
        if (target <= 0.01f || current <= 0.01f)
            return;
        if (Mathf.Abs(current - target) < 0.02f)
            return;

        if (current > target)
        {
            float factor = current / target;
            SpeedMid *= factor;
            SpeedEnd *= factor;
            SpeedShot *= factor;
            PhaseWallMidSeconds /= factor;
            PhaseWallEndSeconds /= factor;
            PhaseWallShotSeconds /= factor;
            _play0 /= factor;
            _play1 /= factor;
            _play2 /= factor;
            AnimationShotAtSeconds = target;
            Debug.Log(
                "[CastSync] ANIM.pinShot speedUp current=" + current.ToString("F3") +
                " → target=" + target.ToString("F3") +
                " factor=" + factor.ToString("F3") +
                " speed=[" + SpeedMid.ToString("F3") + "," + SpeedEnd.ToString("F3") + "," + SpeedShot.ToString("F3") + "]");
            return;
        }

        float deficit = target - current;
        StretchFillerBy(deficit);
        AnimationShotAtSeconds = target;
        Debug.Log(
            "[CastSync] ANIM.pinShot stretchFiller +" + deficit.ToString("F3") +
            " current=" + current.ToString("F3") +
            " → target=" + target.ToString("F3") +
            " filler=" + _fillerIndex +
            " speed=[" + SpeedMid.ToString("F3") + "," + SpeedEnd.ToString("F3") + "," + SpeedShot.ToString("F3") + "]" +
            " walls=[" + PhaseWallMidSeconds.ToString("F3") + "," + PhaseWallEndSeconds.ToString("F3") + "," + PhaseWallShotSeconds.ToString("F3") + "]");
    }

    /// <summary>
    /// Playback lengths from APawn::InitSkillProcess, checked against SkillTiming.log.
    /// cast_style picks the pad. Three-clip letters stretch the middle clip.
    /// Two-clip and Mix stretch the first. Other clips stay at numFrames/30.
    /// A hit time shorter than that chain zeroes the filler and speeds the rest up.
    /// </summary>
    private void ApplyClientCastSpeeds(
        int clipCount,
        float dur0,
        float dur1,
        float dur2,
        float shotEventClipTime,
        int castStyle,
        bool mixCombo)
    {
        float pad = PadSeconds(castStyle);
        // Budget the AttackShot notify to leave time when a projectile flight exists;
        // otherwise keep full HitTime (heal / instant magic).
        float roomBase = FlightTime > 0.001f ? serverTimeToShoot : HitTime;
        float room = Mathf.Max(0.0001f, roomBase - pad);
        float headSlack = ClientChainSlackSeconds;
        float n0 = ClientNativeSeconds(dur0);
        float n1 = ClientNativeSeconds(dur1);
        float n2 = ClientNativeSeconds(dur2);
        float notify = ClientNotifySeconds(shotEventClipTime);
        int filler = clipCount >= 3 && !mixCombo ? 1 : 0;
        int shotIndex = clipCount - 1;

        _clipCount = clipCount;
        _fillerIndex = filler;
        _dur0 = dur0;
        _dur1 = dur1;
        _dur2 = dur2;
        _shotEventClipTime = shotEventClipTime;

        float untilNotify = 0f;
        if (clipCount > 1)
        {
            untilNotify = notify;
            if (0 != filler && 0 < shotIndex)
                untilNotify += n0;
            if (clipCount >= 3 && 1 != filler && 1 < shotIndex)
                untilNotify += n1;
        }

        float fillerInitial = room - (untilNotify + ClientChainSlackSeconds);
        float budget = room - headSlack;
        bool rescale = fillerInitial < 0f && budget > 0.0001f && untilNotify > 0.0001f;
        float scale = rescale ? untilNotify / budget : 1f;

        float play0 = ClipPlaySeconds(0, filler, clipCount, n0, fillerInitial, headSlack, scale, rescale, pad, room);
        float play1 = ClipPlaySeconds(1, filler, clipCount, n1, fillerInitial, headSlack, scale, rescale, pad, room);
        float play2 = ClipPlaySeconds(2, filler, clipCount, n2, fillerInitial, headSlack, scale, rescale, pad, room);
        _play0 = play0;
        _play1 = play1;
        _play2 = play2;

        if (clipCount >= 3)
        {
            SpeedMid = SpeedFor(dur0, play0);
            SpeedEnd = SpeedFor(dur1, play1);
            SpeedShot = SpeedFor(dur2, play2);
        }
        else if (clipCount == 2)
        {
            SpeedEnd = SpeedFor(dur0, play0);
            SpeedShot = SpeedFor(dur1, play1);
            SpeedMid = SpeedEnd;
        }
        else
        {
            float one = SpeedFor(dur0, play0);
            SpeedMid = one;
            SpeedEnd = one;
            SpeedShot = one;
        }

        RebuildWallsFromPlays();

        float beforeShot = clipCount >= 3 ? play0 + play1 : (clipCount == 2 ? play0 : 0f);
        float shotClipLength = clipCount >= 3 ? dur2 : (clipCount == 2 ? dur1 : dur0);
        float shotPlay = clipCount >= 3 ? play2 : (clipCount == 2 ? play1 : play0);
        float shotSpeed = SpeedFor(shotClipLength, shotPlay);
        float eventWall = shotEventClipTime > 0f
            ? shotEventClipTime / Mathf.Max(shotSpeed, 0.0001f)
            : 0f;
        AnimationShotAtSeconds = beforeShot + eventWall;

        Debug.Log(
            "[CastSync] ANIM.setup style=" + castStyle +
            " clips=" + clipCount +
            " pad=" + pad.ToString("F2") +
            " filler=" + filler +
            " rescale=" + rescale +
            " hit=" + HitTime.ToString("F3") +
            " roomBase=" + roomBase.ToString("F3") +
            " shootWin=" + AdjustedShootWindowSeconds.ToString("F3") +
            " flight=" + FlightTime.ToString("F3") +
            " play=[" + play0.ToString("F3") + "," + play1.ToString("F3") + "," + play2.ToString("F3") + "]" +
            " speed=[" + SpeedMid.ToString("F3") + "," + SpeedEnd.ToString("F3") + "," + SpeedShot.ToString("F3") + "]" +
            " walls=[" + PhaseWallMidSeconds.ToString("F3") + "," + PhaseWallEndSeconds.ToString("F3") + "," + PhaseWallShotSeconds.ToString("F3") + "]" +
            " animShotAt=" + AnimationShotAtSeconds.ToString("F3") +
            " sfxShotAt≈" + serverTimeToShoot.ToString("F3"));
    }

    public float GetPhaseEndWallSeconds(int stateIndex)
    {
        if (stateIndex == 0) return PhaseWallMidSeconds;
        if (stateIndex == 1) return PhaseWallEndSeconds;
        if (stateIndex == 2) return PhaseWallShotSeconds;
        return 0f;
    }

    public float GetShotTimeNormalize()
    {
        return Mathf.Max(0, serverTimeToShoot / HitTime);
    }
}
