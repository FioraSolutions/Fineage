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

    public static string FormatMissingSoundError(string notifyId)
    {
        if (!string.IsNullOrEmpty(notifyId)
            && notifyId.IndexOf("AnimNotify_AttackVoice", StringComparison.Ordinal) >= 0)
        {
            return "AnimNotifyPlayback: AnimNotify_AttackVoice authors Volume, Radius, and Random and has no Sound wave. "
                + "No pawn voice binding is in the catalog. id=" + notifyId;
        }

        return "AnimNotifyPlayback: notify has no Sound wave id=" + notifyId;
    }

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

    public static bool TryPlayWave(
        AudioService service,
        string wave,
        Vector3 position,
        float volumeScale,
        float innerRadiusUu,
        out string error,
        string contextHint = null,
        int ownerId = 0,
        int soundSlot = 0,
        string idForLog = null,
        string sourceSystem = null,
        string ownerName = null)
    {
        error = null;
        if (service == null)
        {
            error = "AnimNotifyPlayback: AudioService missing while playing wave=\"" + wave + "\".";
            return false;
        }
        if (string.IsNullOrEmpty(wave))
        {
            error = FormatMissingSoundError("wave=\"\"");
            return false;
        }

        string id = string.IsNullOrEmpty(idForLog) ? wave : idForLog;
        if (OfficialWaveTable.TryResolve(wave, out OfficialWaveTable.Entry official, out string resolveError, contextHint: contextHint)
            && official != null
            && service.PlayClipAtWorld(
                official.ResourcesPath, position, AudioCategory.Sfx, volumeScale, atListener: false,
                innerRadiusUu, ownerId, soundSlot, id, sourceSystem, ownerName))
            return true;
        if (!string.IsNullOrEmpty(resolveError) && resolveError.StartsWith("OfficialWaveTable: ambiguous", System.StringComparison.Ordinal))
        {
            error = resolveError;
            return false;
        }

        for (int i = 0; i < ResourcesRoots.Length; i++)
        {
            string path = ResourcesRoots[i] + wave;
            if (!AudioClipCache.TryLoad(path, Time.time, out _, out _))
                continue;
            if (service.PlayClipAtWorld(
                path, position, AudioCategory.Sfx, volumeScale, atListener: false,
                innerRadiusUu, ownerId, soundSlot, id, sourceSystem, ownerName))
                return true;
        }

        error = resolveError ?? FormatMissingWaveError(wave, "roots=" + string.Join(",", ResourcesRoots));
        return false;
    }

    public static bool TryVolumeScale(float authoredVolume, out float scale)
    {
        if (float.IsNaN(authoredVolume) || float.IsInfinity(authoredVolume) || authoredVolume <= 0f)
        {
            scale = 0f;
            return false;
        }

        scale = authoredVolume / AuthoredVolumeUnit;
        return true;
    }

    public static float VolumeScale(float authoredVolume)
    {
        float scale;
        return TryVolumeScale(authoredVolume, out scale) ? scale : 0f;
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
