using UnityEngine;

/// <summary>
/// Interlude <c>APawn::InitSkillProcess</c> cast timing for one MagicSkillUse.
/// One pipeline for all magic: filler clip stretches to HitTime; other clips stay native
/// (or speed up when the room is short). No freeze-hold, no separate long-cast path.
/// </summary>
public partial class MagicCastData
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

}
