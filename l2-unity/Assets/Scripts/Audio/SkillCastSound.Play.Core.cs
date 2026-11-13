using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    public static void OnMagicSkillUse(Entity caster, MagicSkillUseDto dto)
    {
        if (caster == null || dto == null)
            return;
        if (WeaponChargeSkill.IsCharge(dto.SkillId))
            return;

        EnsureGrpLoaded();
        Vector3 pos = caster.transform.position;
        InterludeSfx.PlaceAtCaster(pos, caster == DomainPacketServices.LocalPlayer);

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
}
