using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class InterludeSfx
{
    public static AudioClip Load(string resourcesPath)
    {
        if (string.IsNullOrEmpty(resourcesPath))
            return null;

        AudioClip clip;
        if (Cache.TryGetValue(resourcesPath, out clip) && clip != null)
            return clip;

        float until;
        if (MissUntil.TryGetValue(resourcesPath, out until) && Time.realtimeSinceStartup < until)
            return null;

        clip = Resources.Load<AudioClip>(resourcesPath);
        if (clip == null)
        {
            MissUntil[resourcesPath] = Time.realtimeSinceStartup + MissRetrySeconds;
            return null;
        }

        MissUntil.Remove(resourcesPath);
        Cache[resourcesPath] = clip;
        return clip;
    }

    public static bool Play(string resourcesPath, float volume = 0.85f)
    {
        AudioClip clip = Load(resourcesPath);
        if (clip == null)
            return false;
        Play(clip, volume);
        return true;
    }

    public static void Play(AudioClip clip, float volume = 0.85f)
    {
        if (clip == null)
            return;
        volume *= GameSettings.Sfx;
        AudioSource src = EnsureOneShotSource();
        if (src == null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main != null
                ? Camera.main.transform.position
                : Vector3.zero, volume);
            return;
        }
        src.PlayOneShot(clip, volume);
    }

    public static void PlayCast(AudioClip clip, float volume = 0.85f)
    {
        if (clip == null)
            return;
        volume *= GameSettings.Sfx;
        AudioSource src = EnsureCastSource();
        if (src == null)
            return;
        src.Stop();
        src.clip = clip;
        src.volume = volume;
        src.loop = false;
        src.Play();
        Debug.Log(
            "[CastSync] AUDIO.castPlay clip=" + clip.name +
            " len=" + clip.length.ToString("F2") +
            " note=ends naturally or StopCast@release");
    }

    public static void StopCast()
    {
        if (_castSrc == null)
            return;
        if (_castSrc.isPlaying)
        {
            Debug.Log(
                "[CastSync] AUDIO.StopCast left=" + CastRemainingSec.ToString("F2") +
                " clip=" + (_castSrc.clip != null ? _castSrc.clip.name : "-"));
        }
        _castSrc.Stop();
    }

    public static void PlayVoice(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return;
        volume *= GameSettings.Sfx;
        AudioSource src = EnsureVoiceSource();
        if (src == null)
            return;
        bool cut = src.isPlaying;
        string prev = src.clip != null ? src.clip.name : "-";
        src.Stop();
        src.clip = clip;
        src.volume = volume;
        src.loop = false;
        src.Play();
        if (cut)
        {
            Debug.Log(
                "[CastSync] AUDIO.voiceReplace cut=" + prev +
                " → " + clip.name);
        }
    }

    public static void StopVoice()
    {
        if (_voiceSrc != null && _voiceSrc.isPlaying)
            _voiceSrc.Stop();
    }
}
