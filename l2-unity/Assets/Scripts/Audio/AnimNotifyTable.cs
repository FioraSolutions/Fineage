using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// Official MeshAnimation notify bindings (time + object + authored waves).
/// Loaded from the lossless ukx dump table; no invented timings.
/// </summary>
public static class AnimNotifyTable
{
    public const string RelativePath = "Data/Meta/AnimNotifies/anim-sequence-notifies.tsv";

    public static Func<string> TextOverride;
    public static Action<string> ErrorLogged;

    static readonly Dictionary<string, List<AnimNotifyBinding>> BySequence =
        new Dictionary<string, List<AnimNotifyBinding>>(StringComparer.OrdinalIgnoreCase);
    static readonly Dictionary<string, List<AnimNotifyBinding>> ByMeshSequence =
        new Dictionary<string, List<AnimNotifyBinding>>(StringComparer.OrdinalIgnoreCase);
    static readonly HashSet<string> SequenceNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    static bool _loaded;
    static string _loadError;

    public static bool IsLoaded => _loaded;
    public static string LoadError => _loadError;
    public static int SequenceCount => BySequence.Count;
    public static int BindingCount { get; private set; }

    public static string FormatMissingTableError(string path) =>
        "AnimNotifyTable: missing notify table path=\"" + path + "\"";

    public static string FormatParseError(string path, int line, string detail) =>
        "AnimNotifyTable: parse error path=\"" + path + "\" line=" + line + " " + detail;

    public static void ClearForTests()
    {
        BySequence.Clear();
        ByMeshSequence.Clear();
        SequenceNames.Clear();
        BindingCount = 0;
        _loaded = false;
        _loadError = null;
        TextOverride = null;
        ErrorLogged = null;
    }

    public static void Load()
    {
        if (_loaded)
            return;
        LoadFrom(ResolveDefaultText(), DefaultPath());
    }

    public static void LoadFrom(string text, string pathForErrors)
    {
        BySequence.Clear();
        ByMeshSequence.Clear();
        SequenceNames.Clear();
        BindingCount = 0;
        _loadError = null;
        _loaded = true;

        if (string.IsNullOrEmpty(text))
        {
            _loadError = FormatMissingTableError(pathForErrors);
            Log(_loadError);
            return;
        }

        int lineNo = 0;
        using (var reader = new StringReader(text))
        {
            string header = reader.ReadLine();
            lineNo++;
            if (header == null)
            {
                _loadError = FormatMissingTableError(pathForErrors);
                Log(_loadError);
                return;
            }

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;
                if (!TryParseRow(line, pathForErrors, lineNo, out AnimNotifyBinding binding))
                    continue;
                BindingCount++;
                SequenceNames.Add(binding.Sequence);
                Add(BySequence, binding.Sequence, binding);
                Add(ByMeshSequence, binding.Animation + "\0" + binding.Sequence, binding);
            }
        }
    }

    public static bool TryGetBySequence(string sequence, out IReadOnlyList<AnimNotifyBinding> bindings)
    {
        EnsureLoaded();
        if (!string.IsNullOrEmpty(sequence) && BySequence.TryGetValue(sequence, out List<AnimNotifyBinding> list))
        {
            bindings = list;
            return list.Count > 0;
        }

        bindings = Array.Empty<AnimNotifyBinding>();
        return false;
    }

    public static bool TryGetByMeshSequence(string animation, string sequence, out IReadOnlyList<AnimNotifyBinding> bindings)
    {
        EnsureLoaded();
        if (!string.IsNullOrEmpty(animation) && !string.IsNullOrEmpty(sequence)
            && ByMeshSequence.TryGetValue(animation + "\0" + sequence, out List<AnimNotifyBinding> list))
        {
            bindings = list;
            return list.Count > 0;
        }

        return TryGetBySequence(sequence, out bindings);
    }

    public static bool HasSequence(string sequence)
    {
        EnsureLoaded();
        return !string.IsNullOrEmpty(sequence) && SequenceNames.Contains(sequence);
    }

    public static IEnumerable<string> SequenceKeys
    {
        get
        {
            EnsureLoaded();
            return SequenceNames;
        }
    }

    static void EnsureLoaded()
    {
        if (!_loaded)
            Load();
    }

    static void Add(Dictionary<string, List<AnimNotifyBinding>> map, string key, AnimNotifyBinding binding)
    {
        if (!map.TryGetValue(key, out List<AnimNotifyBinding> list))
        {
            list = new List<AnimNotifyBinding>();
            map[key] = list;
        }
        list.Add(binding);
    }

    static bool TryParseRow(string line, string path, int lineNo, out AnimNotifyBinding binding)
    {
        binding = null;
        string[] cols = line.Split('\t');
        if (cols.Length < 12)
        {
            Log(FormatParseError(path, lineNo, "expected 12+ columns, got " + cols.Length));
            return false;
        }

        if (!float.TryParse(cols[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float time))
        {
            Log(FormatParseError(path, lineNo, "invalid time=\"" + cols[3] + "\" sequence=\"" + cols[2] + "\""));
            return false;
        }

        string soundRef = cols[9];
        string sound = SoundStem(soundRef);
        bool surfaceStep = cols[11] == "1";
        float volume = ParseOptionalFloat(cols[7], 250f);
        float radius = ParseOptionalFloat(cols[8], 30f);
        Dictionary<string, string[]> arrays = surfaceStep ? ParseSurfaceArrays(cols[10]) : null;
        int frameCount = 0;
        float rate = 0f;
        if (cols.Length > 12)
            int.TryParse(cols[12], NumberStyles.Integer, CultureInfo.InvariantCulture, out frameCount);
        if (cols.Length > 13)
            float.TryParse(cols[13], NumberStyles.Float, CultureInfo.InvariantCulture, out rate);

        binding = new AnimNotifyBinding(
            cols[0], cols[1], cols[2], time, cols[4], cols[5], cols[6],
            volume, radius, sound, soundRef, surfaceStep, arrays, frameCount, rate);
        return true;
    }

    public static string SoundStem(string soundRef)
    {
        if (string.IsNullOrEmpty(soundRef))
            return string.Empty;
        int colon = soundRef.LastIndexOf(':');
        return colon >= 0 && colon < soundRef.Length - 1 ? soundRef.Substring(colon + 1) : soundRef;
    }

    public static Dictionary<string, string[]> ParseSurfaceArrays(string packed)
    {
        var arrays = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(packed))
            return arrays;

        var bags = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        string[] parts = packed.Split(';');
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i];
            int eq = part.IndexOf('=');
            if (eq <= 0)
                continue;
            string key = part.Substring(0, eq);
            string value = part.Substring(eq + 1);
            int bracket = key.IndexOf('[');
            if (bracket > 0)
                key = key.Substring(0, bracket);
            if (string.Equals(key, "Sound", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!bags.TryGetValue(key, out List<string> list))
            {
                list = new List<string>(3);
                bags[key] = list;
            }
            if (!string.IsNullOrEmpty(value))
                list.Add(value);
        }

        foreach (var kv in bags)
            arrays[kv.Key] = kv.Value.ToArray();
        return arrays;
    }

    static float ParseOptionalFloat(string raw, float authoredDefault)
    {
        if (string.IsNullOrEmpty(raw))
            return authoredDefault;
        return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float v)
            ? v
            : authoredDefault;
    }

    static string DefaultPath() =>
        Path.Combine(Application.streamingAssetsPath, RelativePath);

    static string ResolveDefaultText()
    {
        if (TextOverride != null)
            return TextOverride();
        string path = DefaultPath();
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    static void Log(string message)
    {
        ErrorLogged?.Invoke(message);
        Debug.LogError(message);
    }
}
