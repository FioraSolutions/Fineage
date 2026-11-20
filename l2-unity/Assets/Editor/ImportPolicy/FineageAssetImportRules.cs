#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Loads <c>l2-unity/ImportPolicy/asset-import-rules.json</c> and resolves per-asset expectations.
/// </summary>
public static class FineageAssetImportRules
{
    const string RulesAssetPath = "Assets/../ImportPolicy/asset-import-rules.json";

    static RulesDocument _cached;
    static DateTime _cachedWriteUtc;

    public sealed class RulesDocument
    {
        public int version;
        public DurationThresholds audioDurationSeconds;
        public RequiredReadableAsset[] requiredReadableAssets;
        public TextureRule[] textureRules;
        public AudioRule[] audioRules;
    }

    [Serializable]
    public sealed class DurationThresholds
    {
        public float streamingMin = 30f;
        public float compressedInMemoryMin = 3f;
    }

    [Serializable]
    public sealed class RequiredReadableAsset
    {
        public string assetPath;
        public string yamlField;
        public int expectedValue;
        public string reason;
    }

    [Serializable]
    public sealed class TextureRule
    {
        public string id;
        public string[] pathGlobs;
        public string[] filenameGlobs;
        public bool readable;
        public bool mipmaps;
        public bool streamingMipmaps;
        public string filterMode;
        public string npotScale;
        public string wrapMode;
        public PlatformEntry[] platforms;
    }

    [Serializable]
    public sealed class PlatformEntry
    {
        public string name;
        public bool overridden = true;
        public string format;
        public string compression;
        public int maxTextureSize = 2048;
    }

    [Serializable]
    public sealed class AudioRule
    {
        public string id;
        public string[] pathGlobs;
        public string[] filenameGlobs;
        public string loadType;
        public string compressionFormat;
        public float quality;
        public bool loadInBackground;
        public DurationOverrides durationOverrides;
    }

    [Serializable]
    public sealed class DurationOverrides
    {
        public float streamingMinSeconds = 30f;
        public float compressedInMemoryMinSeconds = 3f;
        public float streamingQuality = 0.7f;
        public float compressedInMemoryQuality = 0.65f;
    }

    public sealed class TextureExpectation
    {
        public string RuleId;
        public bool Readable;
        public bool Mipmaps;
        public bool StreamingMipmaps;
        public FilterMode? FilterMode;
        public TextureImporterNPOTScale? NpotScale;
        public TextureWrapMode? WrapMode;
        public Dictionary<string, PlatformEntry> Platforms = new Dictionary<string, PlatformEntry>();
    }

    public sealed class AudioExpectation
    {
        public string RuleId;
        public AudioClipLoadType LoadType;
        public float Quality;
        public AudioCompressionFormat CompressionFormat;
        public bool LoadInBackground;
    }

    static string RulesPathOnDisk()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        return Path.GetFullPath(Path.Combine(projectRoot, "ImportPolicy", "asset-import-rules.json"));
    }

    public static RulesDocument Load()
    {
        string path = RulesPathOnDisk();
        if (!File.Exists(path))
        {
            Debug.LogError("Fineage import policy rules file missing: " + path);
            return null;
        }

        DateTime writeUtc = File.GetLastWriteTimeUtc(path);
        if (_cached != null && writeUtc == _cachedWriteUtc)
            return _cached;

        string json = File.ReadAllText(path);
        var parsed = JsonUtility.FromJson<RulesWire>(json);
        if (parsed == null)
        {
            Debug.LogError("Fineage import policy rules failed to parse: " + path);
            return null;
        }

        _cached = parsed.ToDocument();
        _cachedWriteUtc = writeUtc;
        return _cached;
    }

    public static TextureRule ResolveTextureRule(string assetPath)
    {
        var rules = Load();
        if (rules?.textureRules == null)
            return null;
        assetPath = assetPath.Replace('\\', '/');
        foreach (var rule in rules.textureRules)
        {
            if (RuleMatches(assetPath, rule.pathGlobs, rule.filenameGlobs))
                return rule;
        }
        return null;
    }

    public static AudioRule ResolveAudioRule(string assetPath)
    {
        var rules = Load();
        if (rules?.audioRules == null)
            return null;
        assetPath = assetPath.Replace('\\', '/');
        foreach (var rule in rules.audioRules)
        {
            if (RuleMatches(assetPath, rule.pathGlobs, rule.filenameGlobs))
                return rule;
        }
        return null;
    }

    public static TextureExpectation BuildTextureExpectation(TextureRule rule)
    {
        if (rule == null)
            return null;
        var expectation = new TextureExpectation
        {
            RuleId = rule.id,
            Readable = rule.readable,
            Mipmaps = rule.mipmaps,
            StreamingMipmaps = rule.streamingMipmaps,
        };
        if (!string.IsNullOrEmpty(rule.filterMode))
            expectation.FilterMode = ParseFilterMode(rule.filterMode);
        if (!string.IsNullOrEmpty(rule.npotScale))
            expectation.NpotScale = ParseNpotScale(rule.npotScale);
        if (!string.IsNullOrEmpty(rule.wrapMode))
            expectation.WrapMode = ParseWrapMode(rule.wrapMode);
        if (rule.platforms != null)
        {
            foreach (var platform in rule.platforms)
                expectation.Platforms[platform.name] = platform;
        }
        return expectation;
    }

    public static AudioExpectation BuildAudioExpectation(AudioRule rule, string assetPath)
    {
        if (rule == null)
            return null;

        string loadTypeName = rule.loadType;
        float quality = rule.quality;
        bool loadInBackground = rule.loadInBackground;

        if (rule.durationOverrides != null)
        {
            float duration = EstimateAudioDurationSeconds(assetPath);
            if (duration >= 0f)
            {
                if (duration >= rule.durationOverrides.streamingMinSeconds)
                {
                    loadTypeName = "Streaming";
                    quality = rule.durationOverrides.streamingQuality;
                    loadInBackground = true;
                }
                else if (duration >= rule.durationOverrides.compressedInMemoryMinSeconds)
                {
                    loadTypeName = "CompressedInMemory";
                    quality = rule.durationOverrides.compressedInMemoryQuality;
                    loadInBackground = false;
                }
            }
        }

        return new AudioExpectation
        {
            RuleId = rule.id,
            LoadType = ParseAudioLoadType(loadTypeName),
            Quality = quality,
            CompressionFormat = ParseAudioCompression(rule.compressionFormat),
            LoadInBackground = loadInBackground,
        };
    }

    public static void ApplyTextureRule(TextureImporter importer, TextureExpectation expectation)
    {
        importer.isReadable = expectation.Readable;
        importer.mipmapEnabled = expectation.Mipmaps;
        importer.streamingMipmaps = expectation.StreamingMipmaps;
        if (expectation.FilterMode.HasValue)
            importer.filterMode = expectation.FilterMode.Value;
        if (expectation.NpotScale.HasValue)
            importer.npotScale = expectation.NpotScale.Value;
        if (expectation.WrapMode.HasValue)
        {
            importer.wrapMode = expectation.WrapMode.Value;
        }

        importer.textureCompression = TextureImporterCompression.Compressed;

        foreach (var kvp in expectation.Platforms)
        {
            var platform = kvp.Value;
            var settings = importer.GetPlatformTextureSettings(kvp.Key);
            settings.overridden = platform.overridden;
            settings.maxTextureSize = platform.maxTextureSize;
            settings.format = ParseTextureFormat(platform.format);
            settings.textureCompression = ParseTextureCompression(platform.compression);
            settings.crunchedCompression = false;
            importer.SetPlatformTextureSettings(settings);
        }
    }

    public static void ApplyAudioRule(AudioImporter importer, AudioExpectation expectation)
    {
        var settings = importer.defaultSampleSettings;
        settings.loadType = expectation.LoadType;
        settings.compressionFormat = expectation.CompressionFormat;
        settings.quality = expectation.Quality;
        settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
        importer.defaultSampleSettings = settings;
        importer.loadInBackground = expectation.LoadInBackground;
        importer.preloadAudioData = false;
    }

    static bool RuleMatches(string assetPath, string[] pathGlobs, string[] filenameGlobs)
    {
        if (pathGlobs == null || !pathGlobs.Any(g => GlobMatch(assetPath, g)))
            return false;
        if (filenameGlobs == null || filenameGlobs.Length == 0)
            return true;
        string fileName = Path.GetFileName(assetPath);
        return filenameGlobs.Any(g => GlobMatch(fileName, g));
    }

    static bool GlobMatch(string text, string pattern)
    {
        // fnmatch-style: * only.
        var rx = "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        return System.Text.RegularExpressions.Regex.IsMatch(text, rx, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    static FilterMode ParseFilterMode(string name) =>
        name switch
        {
            "Point" => FilterMode.Point,
            "Bilinear" => FilterMode.Bilinear,
            "Trilinear" => FilterMode.Trilinear,
            _ => throw new ArgumentException("Unknown filterMode: " + name),
        };

    static TextureImporterNPOTScale ParseNpotScale(string name) =>
        name switch
        {
            "None" => TextureImporterNPOTScale.None,
            "ToNearest" => TextureImporterNPOTScale.ToNearest,
            "ToLarger" => TextureImporterNPOTScale.ToLarger,
            "ToSmaller" => TextureImporterNPOTScale.ToSmaller,
            _ => throw new ArgumentException("Unknown npotScale: " + name),
        };

    static TextureWrapMode ParseWrapMode(string name) =>
        name switch
        {
            "Repeat" => TextureWrapMode.Repeat,
            "Clamp" => TextureWrapMode.Clamp,
            "Mirror" => TextureWrapMode.Mirror,
            "MirrorOnce" => TextureWrapMode.MirrorOnce,
            _ => throw new ArgumentException("Unknown wrapMode: " + name),
        };

    static TextureImporterFormat ParseTextureFormat(string name) =>
        name switch
        {
            "RGBA32" => TextureImporterFormat.RGBA32,
            "DXT1" => TextureImporterFormat.DXT1,
            "DXT5" => TextureImporterFormat.DXT5,
            "BC4" => TextureImporterFormat.BC4,
            "BC5" => TextureImporterFormat.BC5,
            "BC6H" => TextureImporterFormat.BC6H,
            "BC7" => TextureImporterFormat.BC7,
            "ASTC_4x4" => TextureImporterFormat.ASTC_4x4,
            "ASTC_5x5" => TextureImporterFormat.ASTC_5x5,
            "ASTC_6x6" => TextureImporterFormat.ASTC_6x6,
            "ASTC_8x8" => TextureImporterFormat.ASTC_8x8,
            "ETC2_RGBA8" => TextureImporterFormat.ETC2_RGBA8,
            _ => throw new ArgumentException("Unknown texture format: " + name),
        };

    static TextureImporterCompression ParseTextureCompression(string name) =>
        name switch
        {
            "Uncompressed" => TextureImporterCompression.Uncompressed,
            "Compressed" => TextureImporterCompression.Compressed,
            "CompressedHQ" => TextureImporterCompression.CompressedHQ,
            _ => throw new ArgumentException("Unknown texture compression: " + name),
        };

    static AudioClipLoadType ParseAudioLoadType(string name) =>
        name switch
        {
            "DecompressOnLoad" => AudioClipLoadType.DecompressOnLoad,
            "CompressedInMemory" => AudioClipLoadType.CompressedInMemory,
            "Streaming" => AudioClipLoadType.Streaming,
            _ => throw new ArgumentException("Unknown audio loadType: " + name),
        };

    static AudioCompressionFormat ParseAudioCompression(string name) =>
        name switch
        {
            "PCM" => AudioCompressionFormat.PCM,
            "Vorbis" => AudioCompressionFormat.Vorbis,
            "ADPCM" => AudioCompressionFormat.ADPCM,
            _ => throw new ArgumentException("Unknown audio compression: " + name),
        };

    static float EstimateAudioDurationSeconds(string assetPath)
    {
        string fullPath = Path.GetFullPath(assetPath);
        if (!File.Exists(fullPath))
            fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
        if (!File.Exists(fullPath))
            return -1f;

        string ext = Path.GetExtension(fullPath).ToLowerInvariant();
        if (ext == ".wav")
            return ReadWavDurationSeconds(fullPath);
        if (ext == ".ogg")
            return ReadOggDurationSeconds(fullPath);
        return -1f;
    }

    static float ReadWavDurationSeconds(string path)
    {
        try
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);
            if (new string(reader.ReadChars(4)) != "RIFF")
                return -1f;
            reader.ReadInt32();
            if (new string(reader.ReadChars(4)) != "WAVE")
                return -1f;
            int sampleRate = 0;
            int channels = 0;
            int bits = 0;
            long dataBytes = 0;
            while (stream.Position < stream.Length)
            {
                string chunkId = new string(reader.ReadChars(4));
                int chunkSize = reader.ReadInt32();
                if (chunkId == "fmt ")
                {
                    reader.ReadInt16();
                    channels = reader.ReadInt16();
                    sampleRate = reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt16();
                    bits = reader.ReadInt16();
                    int remaining = chunkSize - 16;
                    if (remaining > 0)
                        reader.ReadBytes(remaining);
                }
                else if (chunkId == "data")
                {
                    dataBytes = chunkSize;
                    break;
                }
                else
                {
                    stream.Seek(chunkSize, SeekOrigin.Current);
                }
            }
            if (sampleRate <= 0 || channels <= 0 || bits <= 0 || dataBytes <= 0)
                return -1f;
            double bytesPerSecond = sampleRate * channels * (bits / 8.0);
            return (float)(dataBytes / bytesPerSecond);
        }
        catch
        {
            return -1f;
        }
    }

    static float ReadOggDurationSeconds(string path)
    {
        // Best-effort: duration overrides fall back to path rules when unknown.
        return -1f;
    }

    [Serializable]
    sealed class RulesWire
    {
        public int version;
        public DurationThresholds audioDurationSeconds;
        public RequiredReadableAsset[] requiredReadableAssets;
        public TextureRule[] textureRules;
        public AudioRule[] audioRules;

        public RulesDocument ToDocument() => new RulesDocument
        {
            version = version,
            audioDurationSeconds = audioDurationSeconds,
            requiredReadableAssets = requiredReadableAssets,
            textureRules = textureRules,
            audioRules = audioRules,
        };
    }
}
#endif