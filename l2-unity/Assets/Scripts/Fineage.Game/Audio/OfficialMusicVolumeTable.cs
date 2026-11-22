using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

/// <summary>Official MusicVolume actors with world-space brush geometry (L2/UE units).</summary>
public static class OfficialMusicVolumeTable
{
    public const string RelativePath = "Data/Meta/Sounds/official-music-volumes.json.gz";

    [Serializable]
    public class FileData
    {
        public int count;
        public Volume[] volumes;
    }

    [Serializable]
    public class Volume
    {
        public string map;
        public string name;
        public int musicId;
        public bool hasNightMusic;
        public int nightMusicId;
        public bool loopMusic;
        public bool forcePlayMusic;
        public Vector3 min;
        public Vector3 max;
        public Poly[] polys;

        public string Id => map + "/" + name;

        public bool ContainsUePoint(Vector3 uePoint)
        {
            var list = new OfficialBrushSolid.Poly[polys == null ? 0 : polys.Length];
            for (int i = 0; i < list.Length; i++)
            {
                Vector3[] verts = polys[i].vertices ?? Array.Empty<Vector3>();
                list[i] = new OfficialBrushSolid.Poly(polys[i].normal, verts);
            }
            return OfficialBrushSolid.Contains(uePoint, min, max, list);
        }
    }

    [Serializable]
    public class Poly
    {
        public Vector3 normal;
        public Vector3[] vertices;
    }

    public static Func<byte[]> BytesOverride;
    public static Action<string> ErrorLogged;

    static readonly Dictionary<string, List<Volume>> ByMap =
        new Dictionary<string, List<Volume>>(StringComparer.OrdinalIgnoreCase);
    static readonly List<Volume> All = new List<Volume>();
    static bool _loaded;

    public static IReadOnlyList<Volume> AllEntries => All;
    public static int Count => All.Count;
    public static bool HasVolumes => Count > 0;

    public static void ClearForTests()
    {
        ByMap.Clear();
        All.Clear();
        _loaded = false;
        BytesOverride = null;
        ErrorLogged = null;
    }

    public static void Load()
    {
        if (_loaded)
            return;
        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        byte[] bytes = BytesOverride != null ? BytesOverride() : (File.Exists(path) ? File.ReadAllBytes(path) : null);
        LoadFrom(bytes, path);
    }

    public static void LoadFrom(byte[] bytes, string pathForErrors)
    {
        ByMap.Clear();
        All.Clear();
        _loaded = true;
        if (bytes == null || bytes.Length == 0)
        {
            Log("OfficialMusicVolumeTable: missing table path=\"" + pathForErrors + "\"");
            return;
        }

        string json;
        using (var raw = new MemoryStream(bytes, writable: false))
        using (Stream stream = IsGzip(bytes) ? (Stream)new GZipStream(raw, CompressionMode.Decompress) : raw)
        using (var reader = new StreamReader(stream))
            json = reader.ReadToEnd();

        FileData data = JsonUtility.FromJson<FileData>(json);
        if (data == null || data.volumes == null)
        {
            Log("OfficialMusicVolumeTable: parse error path=\"" + pathForErrors + "\"");
            return;
        }

        for (int i = 0; i < data.volumes.Length; i++)
        {
            Volume v = data.volumes[i];
            if (v == null)
                continue;
            All.Add(v);
            if (!ByMap.TryGetValue(v.map ?? "", out List<Volume> list))
            {
                list = new List<Volume>();
                ByMap[v.map ?? ""] = list;
            }
            list.Add(v);
        }
    }

    public static IReadOnlyList<Volume> ForMap(string map)
    {
        EnsureLoaded();
        if (!string.IsNullOrEmpty(map) && ByMap.TryGetValue(map, out List<Volume> list))
            return list;
        return Array.Empty<Volume>();
    }

    public static Volume FindContaining(Vector3 uePoint, string map, string sceneName)
    {
        EnsureLoaded();
        Volume best = null;
        float bestVol = float.MaxValue;
        Consider(map, uePoint, ref best, ref bestVol);
        if (!string.IsNullOrEmpty(sceneName) && !string.Equals(sceneName, map, StringComparison.OrdinalIgnoreCase))
            Consider(sceneName, uePoint, ref best, ref bestVol);
        return best;
    }

    public static Volume FindContainingNearby(Vector3 uePoint, int tileX, int tileY, string sceneName)
    {
        EnsureLoaded();
        Volume best = null;
        float bestVol = float.MaxValue;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
                Consider(VectorUtils.FormatSwMapName(tileX + dx, tileY + dy), uePoint, ref best, ref bestVol);
        }
        if (!string.IsNullOrEmpty(sceneName))
            Consider(sceneName, uePoint, ref best, ref bestVol);
        return best;
    }

    static void Consider(string map, Vector3 uePoint, ref Volume best, ref float bestVol)
    {
        IReadOnlyList<Volume> rows = ForMap(map);
        for (int i = 0; i < rows.Count; i++)
        {
            Volume v = rows[i];
            if (!v.ContainsUePoint(uePoint))
                continue;
            float vol = Mathf.Max(0.01f, (v.max.x - v.min.x) * (v.max.y - v.min.y) * (v.max.z - v.min.z));
            bool better = best == null
                || (v.forcePlayMusic && !best.forcePlayMusic)
                || (v.forcePlayMusic == best.forcePlayMusic && vol < bestVol);
            if (!better)
                continue;
            best = v;
            bestVol = vol;
        }
    }

    static void EnsureLoaded()
    {
        if (!_loaded)
            Load();
    }

    static bool IsGzip(byte[] bytes) => bytes.Length >= 2 && bytes[0] == 0x1f && bytes[1] == 0x8b;

    static void Log(string message)
    {
        ErrorLogged?.Invoke(message);
        Debug.LogError(message);
    }
}
