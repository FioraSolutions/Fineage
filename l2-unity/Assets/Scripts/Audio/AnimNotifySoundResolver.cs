using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Picks the official wave for an AnimNotify_Sound: surface arrays when the
/// notify's primary wave is a stepsound bed, otherwise the authored Sound.
/// Unknown collider tags use the authored Default* array, and the miss is logged once.
/// </summary>
public static class AnimNotifySoundResolver
{
    public const string ResourcesStepSoundRoot = "Data/Sounds/StepSound/";
    public const float AuthoredVolumeUnit = 250f;

    static readonly HashSet<string> LoggedMissingSurface = new HashSet<string>();
    static readonly Dictionary<string, int> VariantCursor = new Dictionary<string, int>(StringComparer.Ordinal);

    public static string FormatMissingWaveError(string wave, string notifyId) =>
        "AnimNotifyPlayback: missing wave=\"" + wave + "\" notify=" + notifyId;

    public static string FormatMissingSoundError(string notifyId) =>
        "AnimNotifyPlayback: notify has no Sound wave id=" + notifyId;

    public static readonly string[] ResourcesRoots =
    {
        ResourcesStepSoundRoot,
        AudioPathResolver.ResourcesItemSoundRoot,
        AudioPathResolver.ResourcesSkillSoundRoot,
        AudioPathResolver.ResourcesSkillVoiceRoot,
        "Data/Sounds/Voice/",
        AudioPathResolver.ResourcesMonSoundRoot,
    };

    public static string ResourcesPathForWave(string wave) =>
        ResourcesPathForWave(wave, surfaceStep: true);

    public static string ResourcesPathForWave(string wave, bool surfaceStep)
    {
        if (string.IsNullOrEmpty(wave))
            return null;
        if (surfaceStep)
            return ResourcesStepSoundRoot + wave;
        return AudioPathResolver.ResourcesItemSoundRoot + wave;
    }

    public static bool TryPlayWave(AudioService service, string wave, Vector3 position, float volumeScale, out string error)
    {
        error = null;
        if (service == null)
        {
            error = "AnimNotifyPlayback: AudioService missing while playing wave="" + wave + "".";
            return false;
        }
        if (string.IsNullOrEmpty(wave))
        {
            error = FormatMissingSoundError("wave=""");
            return false;
        }

        for (int i = 0; i < ResourcesRoots.Length; i++)
        {
            string path = ResourcesRoots[i] + wave;
            if (!AudioClipCache.TryLoad(path, Time.time, out _, out _))
                continue;
            if (service.PlayClipAtWorld(path, position, AudioCategory.Sfx, volumeScale, atListener: false))
                return true;
        }

        error = FormatMissingWaveError(wave, "roots=" + string.Join(",", ResourcesRoots));
        return false;
    }

    public static float VolumeScale(float authoredVolume)
    {
        if (authoredVolume <= 0f)
            return 1f;
        return authoredVolume / AuthoredVolumeUnit;
    }

    public static void ClearForTests()
    {
        LoggedMissingSurface.Clear();
        VariantCursor.Clear();
    }

    public static string ResolveWave(AnimNotifyBinding binding, string surfaceTag, bool running, out string error)
    {
        error = null;
        if (binding == null)
        {
            error = "AnimNotifyPlayback: null notify binding.";
            return null;
        }

        if (!binding.SurfaceStep)
        {
            if (string.IsNullOrEmpty(binding.Sound))
            {
                error = FormatMissingSoundError(binding.Id);
                return null;
            }
            return binding.Sound;
        }

        string prefix = SurfacePrefix(surfaceTag, binding, out string surfaceLog);
        if (surfaceLog != null && LoggedMissingSurface.Add(binding.NotifyName + ":" + (surfaceTag ?? "")))
            error = surfaceLog;

        string arrayName = prefix + (running ? "RunSound" : "WalkSound");
        string[] variants = null;
        if (binding.SurfaceArrays != null)
            binding.SurfaceArrays.TryGetValue(arrayName, out variants);
        if (variants == null || variants.Length == 0)
        {
            string fallbackName = "Default" + (running ? "RunSound" : "WalkSound");
            if (binding.SurfaceArrays != null)
                binding.SurfaceArrays.TryGetValue(fallbackName, out variants);
            if (variants == null || variants.Length == 0)
            {
                if (string.IsNullOrEmpty(binding.Sound))
                {
                    error = "AnimNotifyPlayback: no surface array \"" + arrayName
                        + "\" and no Default/Sound wave notify=" + binding.Id
                        + " surface=\"" + (surfaceTag ?? "") + "\".";
                    return null;
                }
                return binding.Sound;
            }
        }

        return PickVariant(binding.Id + ":" + arrayName, variants);
    }

    public static bool IsRunSequence(string sequence)
    {
        if (string.IsNullOrEmpty(sequence))
            return false;
        return Regex.IsMatch(sequence, @"(^|_)run($|_)", RegexOptions.IgnoreCase);
    }

    static string SurfacePrefix(string surfaceTag, AnimNotifyBinding binding, out string log)
    {
        log = null;
        string tag = string.IsNullOrEmpty(surfaceTag) ? "" : surfaceTag.Trim().ToLowerInvariant();
        switch (tag)
        {
            case "grass":
                return "Grass";
            case "water":
            case "water_shallow":
            case "water_shalow":
            case "water_deep":
                return "Water";
            case "stone":
            case "stone_hard":
            case "defaultactor":
                return "DefaultActor";
            case "dirt":
            case "default":
            case "":
                return "Default";
            default:
                log = "AnimNotifyPlayback: surface tag=\"" + surfaceTag
                    + "\" has no dedicated array on notify=" + binding.Id
                    + ", using authored DefaultWalk/RunSound.";
                return "Default";
        }
    }

    static string PickVariant(string key, string[] variants)
    {
        if (variants.Length == 1)
            return variants[0];
        VariantCursor.TryGetValue(key, out int i);
        string wave = variants[i % variants.Length];
        VariantCursor[key] = i + 1;
        return wave;
    }
}
