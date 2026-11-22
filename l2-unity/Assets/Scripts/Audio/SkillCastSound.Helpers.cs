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
        InterludeSfx.PlaceAtCaster(pos, caster == DomainPacketServices.LocalPlayer);
        bool listener = caster == DomainPacketServices.LocalPlayer;
        bool shotOk = false;
        if (pending.SkillId != 0 || pending.ShotCue.HasClip || (pending.ShotEffects != null && pending.ShotEffects.Length > 0))
        {
            shotOk = PlayCue(pending.ShotCue, "shot", pending.SkillId, pos, listener, asCast: false);
            shotOk = PlayCueList(pending.ShotEffects, "shot", pending.SkillId, pos, listener) || shotOk;
        }
        else if (!string.IsNullOrEmpty(shotClip))
        {
            shotOk = PlayCue(CueFromStem(shotClip), "shot", skillId, pos, listener, asCast: false);
        }

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
            ShotOrbDebug.LogWarning("[ShotOrb] RELEASE force failed: " + orbEx.Message);
        }

        if (caster != null && caster.IsSpiritshotCharged)
            caster.IsSpiritshotCharged = false;
    }

    static void TryPlayExplode(int casterId, Vector3 pos, string reason)
    {
        PendingCast pending;
        if (!_pending.TryGetValue(casterId, out pending) || pending.ExplodePlayed)
            return;
        if (!HasAnyCue(pending.ExplodeCue, pending.ExpEffects) && string.IsNullOrEmpty(pending.ExplodeClip))
            return;
        pending.ExplodePlayed = true;
        _pending[casterId] = pending;
        float t = Time.realtimeSinceStartup - pending.StartTime;
        bool listener = IsLocalCaster(casterId);
        bool ok = PlayCue(pending.ExplodeCue, "exp", pending.SkillId, pos, listener, asCast: false);
        ok = PlayCueList(pending.ExpEffects, "exp", pending.SkillId, pos, listener) || ok;
        if (!ok && !pending.ExplodeCue.HasClip && !string.IsNullOrEmpty(pending.ExplodeClip))
            ok = PlayCue(CueFromStem(pending.ExplodeClip), "exp", pending.SkillId, pos, listener, asCast: false);
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
        PlayCue(CueFromStem(clipName), "cast", 0, Vector3.zero, true, asCast: true, overrideVolume: volume);
    }

    static bool PlayCueList(SkillSoundCue[] cues, string phase, int skillId, Vector3 pos, bool listener)
    {
        if (cues == null || cues.Length == 0)
            return false;
        bool any = false;
        for (int i = 0; i < cues.Length; i++)
            any = PlayCue(cues[i], phase, skillId, pos, listener, asCast: false) || any;
        return any;
    }

    static bool PlayCue(
        SkillSoundCue cue,
        string phase,
        int skillId,
        Vector3 pos,
        bool listener,
        bool asCast,
        float overrideVolume = -1f)
    {
        if (!cue.HasClip)
            return false;

        AudioClip clip = LoadCueClip(cue);
        if (clip == null)
        {
            string package = string.IsNullOrEmpty(cue.Package) ? "?" : cue.Package;
            string name = string.IsNullOrEmpty(cue.Name) ? cue.Clip : cue.Name;
            Debug.LogError(
                "[SkillCastSound] missing clip package=" + package +
                " name=" + name +
                " token=" + (cue.Token ?? "") +
                " skill=" + skillId +
                " phase=" + phase);
            return false;
        }

        float volume = overrideVolume >= 0f ? overrideVolume : VolumeFromTable(cue.Vol);
        if (asCast)
            InterludeSfx.PlayCast(clip, volume);
        else if (listener)
            InterludeSfx.Play(clip, volume);
        else
            InterludeSfx.PlayAt(clip, pos, volume, false, cue.Rad);
        return true;
    }

    static AudioClip LoadCueClip(SkillSoundCue cue)
    {
        string path;
        string error;
        if (TryResolveEffectClip(cue.Package, cue.Name, cue.Clip, out path, out error))
        {
            AudioClip official = InterludeSfx.Load(path);
            if (official != null)
                return official;
        }

        if (string.IsNullOrEmpty(cue.Clip))
            return null;
        AudioClip clip = Load(SfxRoot, cue.Clip);
        if (clip == null)
            clip = Load(SfxRoot, cue.Clip.ToLowerInvariant());
        return clip;
    }

    public static bool TryResolveEffectClip(
        string package,
        string name,
        string clip,
        out string resourcesPath,
        out string error)
    {
        resourcesPath = null;
        error = null;
        string waveName = !string.IsNullOrEmpty(name) ? name : clip;
        if (string.IsNullOrEmpty(waveName))
        {
            error = OfficialWaveTable.FormatMissingWaveError(package, waveName, "empty name");
            return false;
        }

        OfficialWaveTable.Entry entry;
        if (OfficialWaveTable.TryResolve(waveName, out entry, out error, package))
        {
            resourcesPath = entry.ResourcesPath;
            return !string.IsNullOrEmpty(resourcesPath);
        }

        return false;
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
