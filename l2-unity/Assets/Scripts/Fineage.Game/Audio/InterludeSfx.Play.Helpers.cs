using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class InterludeSfx
{
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
