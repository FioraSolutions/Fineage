using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Interlude skill SFX + hero voice from skillsoundgrp / chrsound.
/// <list type="number">
/// <item><description>Start: <c>*_cast</c> SFX + race <c>sub</c> voice (e.g. Element)</description></item>
/// <item><description>Leave (HitTime−flight): <c>*_shot</c> SFX + race <c>throw</c> voice (e.g. Shot)</description></item>
/// <item><description>Impact: <c>*_explotion</c> when projectile hits (if present)</description></item>
/// </list>
/// Table: <c>Resources/Data/SkillSoundGrp.json</c>.
/// </summary>
public static partial class SkillCastSound
{
    const string SfxRoot = "Data/Sounds/SkillSound/";
    const string VoiceRoot = "Data/Sounds/SkillVoice/";
    const string GrpResource = "Data/SkillSoundGrp";

    public static float Volume = 0.8f;
    public static float VoiceVolume = 1f;

    static readonly CharacterRaceAnimation[] RaceSlots =
    {
        CharacterRaceAnimation.MFighter,
        CharacterRaceAnimation.FFighter,
        CharacterRaceAnimation.MDarkElf,
        CharacterRaceAnimation.FDarkElf,
        CharacterRaceAnimation.MDwarf,
        CharacterRaceAnimation.FDwarf,
        CharacterRaceAnimation.MElf,
        CharacterRaceAnimation.FElf,
        CharacterRaceAnimation.MMagic,
        CharacterRaceAnimation.FMagic,
        CharacterRaceAnimation.MOrc,
        CharacterRaceAnimation.FOrc,
        CharacterRaceAnimation.MShaman,
        CharacterRaceAnimation.FShaman,
    };

    static readonly Dictionary<int, string> SkillIdSfx = new Dictionary<int, string>
    {
        { 1011, "heal" }, { 1015, "heal" }, { 1027, "heal" }, { 1216, "heal" },
        { 1217, "greater_heal" }, { 1218, "greater_heal" }, { 1256, "heal" },
        { 1311, "heal" }, { 6847, "heal" },
        { 1177, "wind_strike" }, { 1184, "ice_bolt" }, { 1148, "death_spike" },
        { 1069, "sleep" }, { 1160, "slow" }, { 1164, "curse_weakness" },
        { 1168, "curse_poison" }, { 1221, "curse_fear" }, { 1206, "wind_shackle" },
        { 1040, "shield" }, { 1068, "might" }, { 1013, "recharge" },
        { 1078, "concentration" }, { 1085, "acumen" }, { 1059, "empower" },
        { 1045, "blessed_body" }, { 1048, "blessed_soul" }, { 1240, "guidance" },
        { 1242, "death_whisper" }, { 1238, "freezing_shackle" }, { 1239, "hurricane" },
        { 1235, "hydro_blast" }, { 1230, "blaze" }, { 1267, "shadow_flare" },
        { 1144, "servitor_heal" }, { 1127, "servitor_heal" }, { 1147, "vampiric_touch" },
    };

    static Dictionary<int, SkillSoundRow> _bySkill;
    static Dictionary<long, SkillSoundRow> _bySkillLevel;
    static readonly Dictionary<int, int> _defaultLevel = new Dictionary<int, int>();
    static readonly Dictionary<int, PendingCast> _pending = new Dictionary<int, PendingCast>();
    static int _localReleaseCasterId;
    static bool _grpTried;

    struct SkillSoundRow
    {
        public string Cast;
        public string Shot;
        public string Explode;
        public string[] Kinds;       // cast-time voice (sub)
        public string[] ThrowKinds;  // release-time voice (throw)
    }

    struct PendingCast
    {
        public string ShotClip;
        public string ExplodeClip;
        public string ThrowVoiceKind;
        public bool ShotPlayed;
        public bool ExplodePlayed;
        public float StartTime;
        public float HitTimeMs;
        public float ShotDelaySec;
        public int SkillId;
    }

    public static void OnMagicSkillUse(Entity caster, MagicSkillUseDto dto)
    {
        if (caster == null || dto == null)
            return;
        if (WeaponChargeSkill.IsCharge(dto.SkillId))
            return;

        EnsureGrpLoaded();
        Vector3 pos = caster.transform.position;

        SkillSoundRow row;
        bool fromGrp = TryGetRow(dto.SkillId, dto.SkillLvl, out row);

        string castClip = fromGrp ? row.Cast : null;
        string shotClip = fromGrp ? row.Shot : null;
        string explodeClip = fromGrp ? row.Explode : null;
        // skillsoundgrp voice slots. Scrolls/potions/items have empty kinds — no hero voice.
        // Only invent VoiceKindFor / "Shot" when the skill is missing from the table entirely.
        string castVoice = fromGrp ? KindForRace(row.Kinds, caster) : null;
        string throwVoice = fromGrp ? KindForRace(row.ThrowKinds, caster) : null;

        string key = KeyFor(dto.SkillId, dto.SkillLvl);
        if (string.IsNullOrEmpty(castClip) && !string.IsNullOrEmpty(key))
            castClip = key + "_cast";
        if (string.IsNullOrEmpty(shotClip) && !string.IsNullOrEmpty(key))
            shotClip = key + "_shot";
        if (!fromGrp)
        {
            if (string.IsNullOrEmpty(castVoice))
                castVoice = VoiceKindFor(key ?? StemFromClip(castClip), caster);
            if (string.IsNullOrEmpty(throwVoice))
                throwVoice = "Shot";
        }

        // Packet HitTime ≈ impact; projectile leaves at HitTime − flight.
        // Same ResolveMagicFlightTimeMs as READY_TO_ACT / MagicCastData (not explode-gated).
        Transform targetTf = ResolveTargetTransform(caster, dto.TargetId);
        float flightMs = SetupDurationHelper.ResolveMagicFlightTimeMs(caster, dto.SkillId, targetTf);
        float shotDelay = Mathf.Max(0f, (dto.HitTime - flightMs) / 1000f);

        float castLen = ClipLengthSec(SfxRoot, castClip);
        float shotLen = ClipLengthSec(SfxRoot, shotClip);
        float voiceLen = VoiceClipLengthSec(caster, castVoice);
        float explodeDelay = 0f;
        if (!string.IsNullOrEmpty(explodeClip))
            explodeDelay = Mathf.Max(shotDelay + 0.05f, dto.HitTime / 1000f);
        float hitSec = dto.HitTime / 1000f;

        Debug.Log(
            "[CastSync] PIPELINE.start skill=" + dto.SkillId +
            " lv=" + dto.SkillLvl +
            " hitMs=" + dto.HitTime.ToString("F0") +
            " flightMs=" + flightMs.ToString("F0") +
            " fromGrp=" + fromGrp +
            " key=" + (key ?? "-") +
            "\n  t=0.000  CAST_SFX+VOICE  cast=" + (castClip ?? "-") +
            " len=" + castLen.ToString("F2") +
            " voice=" + (castVoice ?? "-") + " vLen=" + voiceLen.ToString("F2") +
            "\n  t=" + shotDelay.ToString("F3") +
            "  RELEASE  StopCast + SHOT + THROW  shot=" + (shotClip ?? "-") +
            " len=" + shotLen.ToString("F2") +
            " throw=" + (throwVoice ?? "-") +
            "\n  t=" + explodeDelay.ToString("F3") +
            "  EXPLODE  clip=" + (explodeClip ?? "-") +
            "\n  t=" + hitSec.ToString("F3") +
            "  BAR_EMPTY (gauge HitTime)" +
            "\n  note=anim speeds/walls come later in ANIM.setup; compare BAR.tick remainPct vs these t=");

        // 1) Cast SFX + cast-time hero voice (sub / Element / White / …).
        if (!string.IsNullOrEmpty(castClip))
            PlayCastSfx(castClip, Volume);
        PlayVoiceKind(caster, castVoice);

        int casterId = caster.Identity != null ? caster.Identity.Id : 0;
        _pending[casterId] = new PendingCast
        {
            ShotClip = shotClip ?? "",
            ExplodeClip = explodeClip ?? "",
            ThrowVoiceKind = throwVoice ?? "",
            ShotPlayed = false,
            ExplodePlayed = false,
            StartTime = Time.realtimeSinceStartup,
            HitTimeMs = dto.HitTime,
            ShotDelaySec = shotDelay,
            SkillId = dto.SkillId,
        };
        if (caster == DomainPacketServices.LocalPlayer)
            _localReleaseCasterId = casterId;

        string shotCapture = shotClip ?? "";
        string throwCapture = throwVoice ?? "";
        Entity casterCapture = caster;
        int idCapture = casterId;
        InterludeSfx.PlayAfter(shotDelay, () =>
            PlayRelease(idCapture, casterCapture, shotCapture, throwCapture, "timer"));

        // Explode backup if projectile callback missed (impact ≈ HitTime in Fineage timing).
        if (!string.IsNullOrEmpty(explodeClip))
        {
            Vector3 hitPos = targetTf != null ? targetTf.position : pos;
            InterludeSfx.PlayAfter(explodeDelay, () => TryPlayExplode(casterId, hitPos, "timer"));
        }
    }

    public static void NotifyLocalCastBarFinished()
    {
        if (_localReleaseCasterId <= 0)
            return;
        PendingCast pending;
        string shot = "";
        string throwKind = "";
        if (_pending.TryGetValue(_localReleaseCasterId, out pending))
        {
            shot = pending.ShotClip;
            throwKind = pending.ThrowVoiceKind;
            Debug.Log(
                "[CastSync] SFX.castBarDone skill=" + pending.SkillId +
                " t=" + (Time.realtimeSinceStartup - pending.StartTime).ToString("F3") +
                " plannedShot=" + pending.ShotDelaySec.ToString("F3") +
                " alreadyShot=" + pending.ShotPlayed);
        }
        PlayRelease(_localReleaseCasterId, DomainPacketServices.LocalPlayer, shot, throwKind, "castBar");
    }

    public static void OnMagicSkillLaunched(Entity caster, MagicSkillLaunchedDto dto)
    {
        // Launched is often cast-release on L2 packs — explode is projectile impact.
    }

    public static void OnProjectileImpact(int attackerEntityId, Vector3 hitPoint)
    {
        if (attackerEntityId <= 0)
            return;
        TryPlayExplode(attackerEntityId, hitPoint, "projectile");
    }


    [Serializable]
    class SkillSoundGrpDto
    {
        public SkillSoundEntryDto[] entries;
    }

    [Serializable]
    class SkillSoundEntryDto
    {
        public int skillId;
        public int level;
        public string cast;
        public string shot;
        public string explode;
        public string[] kinds;
        public string[] throwKinds;
    }
}
