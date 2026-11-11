using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    static void PlayRelease(int casterId, Entity caster, string shotClip, string throwVoiceKind, string reason)
    {
        PendingCast pending;
        float t = 0f;
        float planned = 0f;
        int skillId = 0;
        if (_pending.TryGetValue(casterId, out pending))
        {
            t = Time.realtimeSinceStartup - pending.StartTime;
            planned = pending.ShotDelaySec;
            skillId = pending.SkillId;
            if (pending.ShotPlayed)
            {
                Debug.Log(
                    "[CastSync] SFX.release SKIP duplicate reason=" + reason +
                    " skill=" + skillId + " t=" + t.ToString("F3"));
                return;
            }
            pending.ShotPlayed = true;
            if (string.IsNullOrEmpty(shotClip))
                shotClip = pending.ShotClip;
            if (string.IsNullOrEmpty(throwVoiceKind))
                throwVoiceKind = pending.ThrowVoiceKind;
            _pending[casterId] = pending;
        }

        bool castWasPlaying = InterludeSfx.IsCastPlaying;
        float castLeft = InterludeSfx.CastRemainingSec;
        InterludeSfx.StopCast();

        Vector3 pos = caster != null ? caster.transform.position : Vector3.zero;
        bool shotOk = false;
        if (!string.IsNullOrEmpty(shotClip))
            shotOk = PlaySfx(shotClip, pos, Volume);

        // 2) Release hero voice — PlayVoice cuts any still-playing cast line.
        PlayVoiceKind(caster, throwVoiceKind);

        float barRem = CastBarOverlay.Remaining01;
        Debug.Log(
            "[CastSync] PIPELINE.hit RELEASE reason=" + reason +
            " skill=" + skillId +
            " t=" + t.ToString("F3") +
            " planned=" + planned.ToString("F3") +
            " drift=" + (t - planned).ToString("F3") +
            " barRemainPct=" + (barRem < 0f ? -1f : barRem * 100f).ToString("F0") +
            " StopCast wasPlaying=" + castWasPlaying +
            " leftSec=" + castLeft.ToString("F2") +
            " shot=" + (shotClip ?? "-") + " ok=" + shotOk +
            " voiceThrow=" + (throwVoiceKind ?? "-"));

        // Flying skill orb (el_*_fl) — CastClip AnimationShoot often never reaches V2.
        try
        {
            MagicSkillOrbLaunch.TryForceAtRelease(caster, skillId);
        }
        catch (Exception orbEx)
        {
            Debug.LogWarning("[ShotOrb] RELEASE force failed: " + orbEx.Message);
        }

        if (caster != null && caster.IsSpiritshotCharged)
            caster.IsSpiritshotCharged = false;
    }

    static void TryPlayExplode(int casterId, Vector3 pos, string reason)
    {
        PendingCast pending;
        if (!_pending.TryGetValue(casterId, out pending) || pending.ExplodePlayed)
            return;
        if (string.IsNullOrEmpty(pending.ExplodeClip))
            return;
        pending.ExplodePlayed = true;
        _pending[casterId] = pending;
        float t = Time.realtimeSinceStartup - pending.StartTime;
        bool ok = PlaySfx(pending.ExplodeClip, pos, Volume);
        float barRem = CastBarOverlay.Remaining01;
        Debug.Log(
            "[CastSync] PIPELINE.hit EXPLODE reason=" + reason +
            " skill=" + pending.SkillId +
            " t=" + t.ToString("F3") +
            " hitMs=" + pending.HitTimeMs.ToString("F0") +
            " barRemainPct=" + (barRem < 0f ? -1f : barRem * 100f).ToString("F0") +
            " clip=" + pending.ExplodeClip +
            " ok=" + ok);
    }

    static float ClipLengthSec(string root, string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return 0f;
        AudioClip clip = Load(root, clipName);
        if (clip == null)
            clip = Load(root, clipName.ToLowerInvariant());
        return clip != null ? clip.length : 0f;
    }

    static float VoiceClipLengthSec(Entity caster, string kind)
    {
        string prefix = VoicePrefixFor(caster);
        if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(kind))
            return 0f;
        AudioClip clip = Load(VoiceRoot, prefix + "_" + kind);
        return clip != null ? clip.length : 0f;
    }

    static Transform ResolveTargetTransform(Entity caster, int targetId)
    {
        if (targetId > 0)
        {
            World world = WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
            Entity target = world != null ? world.GetEntityNoLockSync(targetId) : null;
            if (target != null)
                return target.transform;
        }
        return caster != null ? caster.Target : null;
    }

    static void PlayCastSfx(string clipName, float volume)
    {
        AudioClip clip = Load(SfxRoot, clipName);
        if (clip == null && !string.IsNullOrEmpty(clipName))
            clip = Load(SfxRoot, clipName.ToLowerInvariant());
        if (clip == null)
            return;
        InterludeSfx.PlayCast(clip, volume);
    }

    static string StemFromClip(string clip)
    {
        if (string.IsNullOrEmpty(clip))
            return null;
        if (clip.EndsWith("_cast", StringComparison.Ordinal))
            return clip.Substring(0, clip.Length - 5);
        if (clip.EndsWith("_shot", StringComparison.Ordinal))
            return clip.Substring(0, clip.Length - 5);
        return clip;
    }

}
