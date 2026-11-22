using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using UnityEngine;

/// <summary>
/// Official AmbientSoundObject / AmbientSound placement from Interlude map dumps.
/// </summary>
public static class OfficialAmbientSoundTable
{
    public const string RelativePath = "Data/Meta/Sounds/official-ambient-objects.tsv.gz";
    public const string RelativePathUncompressed = "Data/Meta/Sounds/official-ambient-objects.tsv";

    public readonly struct Entry
    {
        public readonly string Map;
        public readonly string Name;
        public readonly string Sound;
        public readonly Vector3 UePosition;
        public readonly float RadiusUu;
        public readonly float Volume;
        public readonly float Pitch;
        public readonly AmbientSoundType Type;
        public readonly int Random;
        public readonly float StartTime;

        public Entry(
            string map,
            string name,
            string sound,
            Vector3 uePosition,
            float radiusUu,
            float volume,
            float pitch,
            AmbientSoundType type,
            int random,
            float startTime)
        {
            Map = map;
            Name = name;
            Sound = sound;
            UePosition = uePosition;
            RadiusUu = radiusUu;
            Volume = volume;
            Pitch = pitch;
            Type = type;
            Random = random;
            StartTime = startTime;
        }

        public Vector3 UnityPosition => VectorUtils.ConvertPosToUnity(UePosition);
        public float RadiusUnity => OfficialAmbientSoundUnits.RadiusToUnity(RadiusUu);
        public string Id => Map + "/" + Name;
    }

    public static Func<byte[]> BytesOverride;
    public static Action<string> ErrorLogged;

    static readonly Dictionary<string, List<Entry>> ByMap =
        new Dictionary<string, List<Entry>>(StringComparer.OrdinalIgnoreCase);
    static readonly List<Entry> All = new List<Entry>();
    static bool _loaded;
    static string _loadError;

    public static IReadOnlyList<Entry> AllEntries => All;
    public static int Count => All.Count;
    public static bool HasActors => Count > 0;
    public static string LoadError => _loadError;

    public static string FormatMissingTableError(string path) =>
        "OfficialAmbientSoundTable: missing ambient object table path=\"" + path + "\"";

    public static string FormatParseError(string path, int line, string detail) =>
        "OfficialAmbientSoundTable: parse error path=\"" + path + "\" line=" + line + " " + detail;

    public static void ClearForTests()
    {
        ByMap.Clear();
        All.Clear();
        _loaded = false;
        _loadError = null;
        BytesOverride = null;
        ErrorLogged = null;
    }

    public static void Load()
    {
        if (_loaded)
            return;
        string path;
        byte[] bytes = ResolveDefaultBytes(out path);
        LoadFrom(bytes, path);
    }

    public static void LoadFrom(byte[] bytes, string pathForErrors)
    {
        ByMap.Clear();
        All.Clear();
        _loadError = null;
        _loaded = true;
        if (bytes == null || bytes.Length == 0)
        {
            _loadError = FormatMissingTableError(pathForErrors);
            Log(_loadError);
            return;
        }

        using (var stream = OpenText(bytes, pathForErrors))
        using (var reader = new StreamReader(stream))
        {
            string header = reader.ReadLine();
            if (header == null)
            {
                _loadError = FormatMissingTableError(pathForErrors);
                Log(_loadError);
                return;
            }

            string[] cols = header.Split('\t');
            int mapIdx = IndexOf(cols, "map");
            int nameIdx = IndexOf(cols, "name");
            int soundIdx = IndexOf(cols, "sound");
            int xIdx = IndexOf(cols, "x");
            int yIdx = IndexOf(cols, "y");
            int zIdx = IndexOf(cols, "z");
            int radiusIdx = IndexOf(cols, "radius");
            int volumeIdx = IndexOf(cols, "volume");
            int pitchIdx = IndexOf(cols, "pitch");
            int typeIdx = IndexOf(cols, "type");
            int randomIdx = IndexOf(cols, "random");
            int startIdx = IndexOf(cols, "start");
            if (mapIdx < 0 || soundIdx < 0 || xIdx < 0 || yIdx < 0 || zIdx < 0)
            {
                _loadError = FormatParseError(pathForErrors, 1, "expected map, sound, x, y, z columns");
                Log(_loadError);
                return;
            }

            string line;
            int lineNo = 1;
            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;
                string[] cells = line.Split('\t');
                if (cells.Length <= Math.Max(Math.Max(mapIdx, soundIdx), zIdx))
                {
                    Log(FormatParseError(pathForErrors, lineNo, "truncated row"));
                    continue;
                }

                AmbientSoundType type = AmbientSoundType.AST1_Always;
                string typeRaw = Cell(cells, typeIdx);
                if (!OfficialAmbientSoundUnits.TryParseType(typeRaw, out type))
                {
                    Log(FormatParseError(pathForErrors, lineNo, "unknown AmbientSoundType=\"" + typeRaw + "\""));
                    type = AmbientSoundType.AST1_Always;
                }

                var entry = new Entry(
                    Cell(cells, mapIdx),
                    Cell(cells, nameIdx),
                    Cell(cells, soundIdx),
                    new Vector3(Float(cells, xIdx), Float(cells, yIdx), Float(cells, zIdx)),
                    Float(cells, radiusIdx),
                    Float(cells, volumeIdx),
                    Float(cells, pitchIdx),
                    type,
                    (int)Float(cells, randomIdx),
                    Float(cells, startIdx));
                All.Add(entry);
                if (!ByMap.TryGetValue(entry.Map, out List<Entry> list))
                {
                    list = new List<Entry>();
                    ByMap[entry.Map] = list;
                }
                list.Add(entry);
            }
        }
    }

    public static void LoadFrom(string text, string pathForErrors)
    {
        byte[] bytes = string.IsNullOrEmpty(text) ? null : System.Text.Encoding.UTF8.GetBytes(text);
        LoadFrom(bytes, pathForErrors);
    }

    public static IReadOnlyList<Entry> ForMap(string map)
    {
        EnsureLoaded();
        if (!string.IsNullOrEmpty(map) && ByMap.TryGetValue(map, out List<Entry> list))
            return list;
        return Array.Empty<Entry>();
    }

    static void EnsureLoaded()
    {
        if (!_loaded)
            Load();
    }

    static Stream OpenText(byte[] bytes, string pathForErrors)
    {
        var raw = new MemoryStream(bytes, writable: false);
        if (IsGzip(bytes) || (pathForErrors != null && pathForErrors.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)))
            return new GZipStream(raw, CompressionMode.Decompress);
        return raw;
    }

    static bool IsGzip(byte[] bytes) => bytes.Length >= 2 && bytes[0] == 0x1f && bytes[1] == 0x8b;

    static byte[] ResolveDefaultBytes(out string path)
    {
        if (BytesOverride != null)
        {
            path = "override";
            return BytesOverride();
        }

        string gz = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (File.Exists(gz))
        {
            path = gz;
            return File.ReadAllBytes(gz);
        }

        string tsv = Path.Combine(Application.streamingAssetsPath, RelativePathUncompressed);
        if (File.Exists(tsv))
        {
            path = tsv;
            return File.ReadAllBytes(tsv);
        }

        path = gz;
        return null;
    }

    static int IndexOf(string[] cols, string name)
    {
        for (int i = 0; i < cols.Length; i++)
        {
            if (string.Equals(cols[i], name, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    static string Cell(string[] cells, int idx) =>
        idx >= 0 && idx < cells.Length ? cells[idx] : "";

    static float Float(string[] cells, int idx)
    {
        string raw = Cell(cells, idx);
        if (string.IsNullOrEmpty(raw))
            return 0f;
        return float.Parse(raw, CultureInfo.InvariantCulture);
    }

    static void Log(string message)
    {
        if (ErrorLogged != null)
            ErrorLogged(message);
        else
            Debug.LogError(message);
    }
}
