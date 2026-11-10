using UnityEngine;

/// <summary>
/// Interlude <c>APawn::InitSkillProcess</c> cast timing for one MagicSkillUse.
/// One pipeline for all magic: filler clip stretches to HitTime; other clips stay native
/// (or speed up when the room is short). No freeze-hold, no separate long-cast path.
/// </summary>
public class MagicCastData
{
    /// <summary>
    /// <b>false</b> — считать <see cref="AnimatorWallPenaltySeconds"/> по кривой ниже (<c>AdjustedShootWindow = serverTimeToShoot − penalty</c>).
    /// <para>
    /// <b>true</b> (сейчас): кривая wall penalty отключена — при fixed duration тайминг клипа уже стабильный,
    /// используем только фиксированный <see cref="CastShootWindowFixedOffsetSeconds"/>.
    /// Код penalty в <c>else</c> намеренно не удалён, чтобы можно было вернуть без восстановления из git.
    /// </para>
    /// </summary>
    private static bool MagicCastWallPenaltyDisabledForTesting = true;

    /// <summary>
    /// Фиксированный сдвиг окна до выстрела (сек): вычитается из номинального <c>serverTimeToShoot</c>,
    /// как раньше «штраф», но постоянный (~150 мс) — выше <c>timingScale</c>, событие выстрела чуть раньше по реальному времени.
    /// </summary>
    private const float CastShootWindowFixedOffsetSeconds = 0.15f;

    /// <summary>При коротком server HitTime (после клампа) — нижнее значение штрафа, сек (~70 мс).</summary>
    private const float WallPenaltyClampHitMinSeconds = 2f;

    /// <summary>При длинном server HitTime (после клампа) — верх штрафа, сек (~150 мс при старой линейной модели).</summary>
    private const float WallPenaltyClampHitMaxSeconds = 8f;

    private const float WallPenaltySecondsMin = 0.07f;
    private const float WallPenaltySecondsMax = 0.15f;

    /// <summary>
    /// Imported clips are keyed at 24 fps and stop on the last key, so length is (frames-1)/24.
    /// The client plays numFrames/30.
    /// </summary>
    private const float ClientFramesPerSecond = 30f;
    private const float UnityClipFramesPerSecond = 24f;
    /// <summary>Added to the chain before the filler is sized. InitSkillProcess +518 at cast speed 1.</summary>
    private const float ClientChainSlackSeconds = 0.2f;
    /// <summary>First clip is shortened by this over (hit - pad) when pad is at least this large.</summary>
    private const float ClientFirstClipFactorSeconds = 0.15f;

    public float StartTime;
    public float HitTime;
    public float FlightTime;

    /// <summary>
    /// Wall-clock skill animation duration (e.g. SpAtk until SwitchToIdle).
    /// When &gt; 0 and composite has Match Lifetime To Skill Animation, FX lifetime uses this instead of HitTime.
    /// </summary>
    public float SkillAnimationDuration;

    /// <summary>Неучтённые в таймлинке переходы/оверлапы (масштаб от server HitTime), только если penalty включён.</summary>
    public float AnimatorWallPenaltySeconds { get; private set; }

    public float SpeedMid = 1.0f;
    public float SpeedEnd = 1.0f;
    public float SpeedShot = 1.0f;
    public float shotEventTime;
    public float serverTimeToShoot;

    /// <summary>
    /// Seconds from cast start to the AttackShot frame on the last clip.
    /// The projectile fallback uses this. serverTimeToShoot (HitTime − flight) moves with distance.
    /// </summary>
    public float AnimationShotAtSeconds;

    /// <summary>
    /// Interlude: AttackShot notify name is None / Mag seq missing → SkillEffectFinalize
    /// TriggerShot on the first MagicProcess tick with TriggerCasting. Spawn ShotAction now.
    /// </summary>
    public bool SpawnShotVisualsImmediately;

    /// <summary>
    /// Client cumulative clip walls from cast start. MagicProcess advances the
    /// sequence on these absolute walls, not clip-local completion times.
    /// </summary>
    public float PhaseWallMidSeconds { get; private set; }
    public float PhaseWallEndSeconds { get; private set; }
    public float PhaseWallShotSeconds { get; private set; }

    /// <summary>Object id цели из пакета MagicSkillUseDto (приоритетнее PlayerEntity.TargetId для VFX).</summary>
    public int TargetObjectId;

    /// <summary>Окно от старта каста до момента выстрела: номинал минус penalty или минус фиксированный offset.</summary>
    public float AdjustedShootWindowSeconds { get; private set; }

    public float DurMid;
    public float DurEnd;
    public float DurShot;

    int _clipCount;
    int _fillerIndex;
    float _dur0;
    float _dur1;
    float _dur2;
    float _shotEventClipTime;
    float _play0;
    float _play1;
    float _play2;

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
    /// Late → speed up the whole chain. Early → stretch the filler clip further (InitSkillProcess),
    /// never freeze the animator.
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

    void StretchFillerBy(float extraSeconds)
    {
        if (extraSeconds <= 0.0001f || _clipCount <= 0)
            return;

        if (_fillerIndex == 0)
        {
            _play0 += extraSeconds;
            SpeedMid = SpeedFor(_dur0, _play0);
            if (_clipCount == 2)
                SpeedEnd = SpeedMid;
            if (_clipCount == 1)
            {
                SpeedEnd = SpeedMid;
                SpeedShot = SpeedMid;
            }
        }
        else if (_fillerIndex == 1)
        {
            _play1 += extraSeconds;
            SpeedEnd = SpeedFor(_dur1, _play1);
        }
        else if (_fillerIndex == 2)
        {
            _play2 += extraSeconds;
            SpeedShot = SpeedFor(_dur2, _play2);
        }

        RebuildWallsFromPlays();
    }

    void RebuildWallsFromPlays()
    {
        if (_clipCount >= 3)
        {
            PhaseWallMidSeconds = _play0;
            PhaseWallEndSeconds = _play0 + _play1;
            PhaseWallShotSeconds = _play0 + _play1 + _play2;
        }
        else if (_clipCount == 2)
        {
            PhaseWallMidSeconds = 0f;
            PhaseWallEndSeconds = _play0;
            PhaseWallShotSeconds = _play0 + _play1;
        }
        else
        {
            PhaseWallMidSeconds = 0f;
            PhaseWallEndSeconds = _play0;
            PhaseWallShotSeconds = _play0;
        }
    }

    /// <summary>
    /// Playback lengths from APawn::InitSkillProcess, checked against SkillTiming.log.
    /// cast_style picks the pad (2/5/8/10 = 0.4, 14 = 2, 3/12 = 0, otherwise 0.15).
    /// Three-clip letters A–I stretch the middle clip. Two-clip J/K/L and Mix stretch the first.
    /// Other clips stay at numFrames/30. A hit time shorter than that chain zeroes the filler
    /// and speeds the rest up. pawn+6B8 was 1.0 on the logged character, so it is not applied again.
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

    private static float ClipPlaySeconds(
        int index,
        int filler,
        int clipCount,
        float native,
        float fillerInitial,
        float headSlack,
        float scale,
        bool rescale,
        float pad,
        float room)
    {
        if (index >= clipCount || native <= 0f)
            return 0f;

        float play;
        if (index == filler)
        {
            if (rescale)
                play = 0f;
            else if (index == 0)
                play = fillerInitial + headSlack;
            else
                play = fillerInitial;
        }
        else if (index == 0 && clipCount > 1)
        {
            float baseSeconds = native / scale + headSlack;
            bool rewriteFirstClip = pad >= ClientFirstClipFactorSeconds;
            play = rewriteFirstClip
                ? baseSeconds * (1f - ClientFirstClipFactorSeconds / room)
                : baseSeconds;
        }
        else
        {
            play = native / scale;
        }

        return play > 0f ? play : 0f;
    }

    /// <summary>Unity clip length / wall play seconds. Filler may be &lt; ×1 (InitSkillProcess stretch).</summary>
    private static float SpeedFor(float unityLength, float playSeconds)
    {
        if (unityLength <= 0f)
            return 1f;
        if (playSeconds <= 0.0001f)
            return unityLength / 0.0001f;
        return unityLength / playSeconds;
    }

    /// <summary>skillgrp cast_style, stored by the client at pawn+52C.</summary>
    private static float PadSeconds(int castStyle)
    {
        switch (castStyle)
        {
            case 2:
            case 5:
            case 8:
            case 10:
                return 0.4f;
            case 14:
                return 2f;
            case 3:
            case 12:
                return 0f;
            default:
                return 0.15f;
        }
    }

    private static float ClientNativeSeconds(float unityLength)
    {
        if (unityLength <= 0f)
            return 0f;
        return (unityLength * UnityClipFramesPerSecond + 1f) / ClientFramesPerSecond;
    }

    private static float ClientNotifySeconds(float unityEventTime)
    {
        if (unityEventTime <= 0f)
            return 0f;
        return unityEventTime * UnityClipFramesPerSecond / ClientFramesPerSecond;
    }

    public float GetShotTimeNormalize()
    {
        return Mathf.Max(0, serverTimeToShoot / HitTime);
    }

    private static float GetDurationSafe(float[] clipsDurations, int index)
    {
        if (clipsDurations == null || index < 0 || index >= clipsDurations.Length)
            return 0f;
        return Mathf.Max(0f, clipsDurations[index]);
    }
}
