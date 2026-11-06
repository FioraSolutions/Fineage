using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single Unity 2D SFX hub for Interlude clips under Resources/Data/Sounds/*.
/// FMOD AudioManager stays disabled — skills, monsters, hits, charge, level-up all go here.
/// Cast and one-shot use separate AudioSources so StopCast cannot kill *_shot / voice.
/// </summary>
public static class InterludeSfx
{
    const string HostName = "FineageInterludeSfx2D";
    static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();
    static readonly Dictionary<string, float> MissUntil = new Dictionary<string, float>();
    const float MissRetrySeconds = 2f;

    static AudioSource _oneShotSrc;
    static AudioSource _castSrc;
    static InterludeSfxRunner _runner;

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
    /// Replaceable cast hold clip on its own source. <see cref="StopCast"/> must not touch one-shots.
    /// </summary>
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
        src.Play();
    }

    public static void StopCast()
    {
        if (_castSrc != null)
            _castSrc.Stop();
    }

    /// <summary>Delayed play without depending on disabled FMOD AudioManager.</summary>
    public static void PlayAfter(float delaySeconds, System.Action action)
    {
        if (action == null)
            return;
        if (delaySeconds <= 0f)
        {
            action();
            return;
        }
        InterludeSfxRunner runner = EnsureRunner();
        if (runner == null)
        {
            action();
            return;
        }
        runner.StartCoroutine(DelayThen(delaySeconds, action));
    }

    static IEnumerator DelayThen(float delay, System.Action action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action();
    }

    static AudioSource EnsureOneShotSource()
    {
        EnsureRunner();
        if (_oneShotSrc != null)
            return _oneShotSrc;
        _oneShotSrc = CreateChildSource("OneShot");
        return _oneShotSrc;
    }

    static AudioSource EnsureCastSource()
    {
        EnsureRunner();
        if (_castSrc != null)
            return _castSrc;
        _castSrc = CreateChildSource("Cast");
        return _castSrc;
    }

    static AudioSource CreateChildSource(string childName)
    {
        if (_runner == null)
            return null;
        Transform existing = _runner.transform.Find(childName);
        GameObject go = existing != null ? existing.gameObject : new GameObject(childName);
        if (existing == null)
            go.transform.SetParent(_runner.transform, false);
        AudioSource src = go.GetComponent<AudioSource>();
        if (src == null)
            src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        src.loop = false;
        src.volume = 1f;
        return src;
    }

    static InterludeSfxRunner EnsureRunner()
    {
        if (_runner != null)
            return _runner;
        GameObject go = GameObject.Find(HostName);
        if (go == null)
        {
            go = new GameObject(HostName);
            Object.DontDestroyOnLoad(go);
        }
        _runner = go.GetComponent<InterludeSfxRunner>();
        if (_runner == null)
            _runner = go.AddComponent<InterludeSfxRunner>();
        return _runner;
    }

    sealed class InterludeSfxRunner : MonoBehaviour { }
}
