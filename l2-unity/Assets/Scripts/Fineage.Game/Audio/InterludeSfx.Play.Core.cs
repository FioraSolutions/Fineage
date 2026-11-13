using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class InterludeSfx
{
    /// <summary>Full volume inside this radius. About 100 L2 units.</summary>
    public const float WorldHearMinMeters = 2f;

    /// <summary>Silent past this. 28 Unity meters is about 1500 L2 units, a street, not the town.</summary>
    public const float WorldHearMaxMeters = 28f;

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

    /// <summary>
    /// One-shot at a world position. Own clips stay on the 2D bus.
    /// A fresh source so two remote shots do not share one transform.
    /// </summary>
    public static void PlayAt(AudioClip clip, Vector3 worldPos, float volume, bool listener)
    {
        if (clip == null)
            return;
        if (listener)
        {
            Play(clip, volume);
            return;
        }

        volume *= GameSettings.Sfx;
        if (volume <= 0f)
            return;

        GameObject go = new GameObject("SfxWorld");
        go.transform.position = worldPos;
        InterludeSfxRunner runner = EnsureRunner();
        if (runner != null)
            go.transform.SetParent(runner.transform, true);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        ApplyWorldFalloff(src);
        src.PlayOneShot(clip, volume);
        UnityEngine.Object.Destroy(go, clip.length + 0.15f);
    }
}
