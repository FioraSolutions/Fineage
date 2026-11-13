using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class InterludeSfx
{
    static AudioSource EnsureCastSource()
    {
        EnsureRunner();
        if (_castSrc != null)
            return _castSrc;
        _castSrc = CreateChildSource("Cast");
        return _castSrc;
    }

    static AudioSource EnsureVoiceSource()
    {
        EnsureRunner();
        if (_voiceSrc != null)
            return _voiceSrc;
        _voiceSrc = CreateChildSource("Voice");
        return _voiceSrc;
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

    /// <summary>
    /// Own casts stay on the listener. Everyone else hears the clip at the caster.
    /// The shared one-shot bus stays 2D so hits and UI are not pulled to the last caster.
    /// </summary>
    public static void PlaceAtCaster(Vector3 worldPos, bool listener)
    {
        Place(EnsureCastSource(), worldPos, listener);
        Place(EnsureVoiceSource(), worldPos, listener);
    }

    static void Place(AudioSource src, Vector3 worldPos, bool listener)
    {
        if (src == null)
            return;
        if (listener)
        {
            src.spatialBlend = 0f;
            src.transform.localPosition = Vector3.zero;
            return;
        }

        ApplyWorldFalloff(src);
        src.transform.position = worldPos;
    }

    internal static void ApplyWorldFalloff(AudioSource src)
    {
        src.spatialBlend = 1f;
        src.dopplerLevel = 0f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = WorldHearMinMeters;
        src.maxDistance = WorldHearMaxMeters;
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
}
