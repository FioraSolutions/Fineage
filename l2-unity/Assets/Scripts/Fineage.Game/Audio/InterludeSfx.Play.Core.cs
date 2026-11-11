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
}
