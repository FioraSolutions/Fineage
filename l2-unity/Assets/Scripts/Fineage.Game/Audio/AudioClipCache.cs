using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Resources clip cache — no per-play load; logs concrete miss id (testable loader hook).</summary>
public static class AudioClipCache
{
    static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();
    static readonly Dictionary<string, float> MissUntil = new Dictionary<string, float>();

    public const float MissRetrySeconds = 2f;

    /// <summary>Editor tests inject a fake loader (no Resources I/O).</summary>
    public static Func<string, AudioClip> LoaderOverride;

    public static string FormatMissingClipError(string resourcesPath) =>
        "AudioService: missing Resources clip id=\"" + resourcesPath + "\"";

    public static bool TryGetCached(string resourcesPath, out AudioClip clip) =>
        Cache.TryGetValue(resourcesPath, out clip) && clip != null;

    public static void PutCache(string resourcesPath, AudioClip clip)
    {
        if (string.IsNullOrEmpty(resourcesPath) || clip == null)
            return;
        MissUntil.Remove(resourcesPath);
        Cache[resourcesPath] = clip;
    }

    public static bool IsMissCoolingDown(string resourcesPath, float now)
    {
        float until;
        return MissUntil.TryGetValue(resourcesPath, out until) && now < until;
    }

    public static void MarkMiss(string resourcesPath, float now) =>
        MissUntil[resourcesPath] = now + MissRetrySeconds;

    public static bool TryLoad(string resourcesPath, float now, out AudioClip clip, out string error)
    {
        error = null;
        clip = null;
        if (string.IsNullOrEmpty(resourcesPath))
        {
            error = "AudioService: empty Resources clip id.";
            return false;
        }

        if (TryGetCached(resourcesPath, out clip))
            return true;

        if (IsMissCoolingDown(resourcesPath, now))
        {
            error = FormatMissingClipError(resourcesPath);
            return false;
        }

        clip = LoaderOverride != null
            ? LoaderOverride(resourcesPath)
            : Resources.Load<AudioClip>(resourcesPath);

        if (clip == null)
        {
            MarkMiss(resourcesPath, now);
            error = FormatMissingClipError(resourcesPath);
            return false;
        }

        PutCache(resourcesPath, clip);
        return true;
    }

    public static void ClearForTests()
    {
        Cache.Clear();
        MissUntil.Clear();
        LoaderOverride = null;
    }
}