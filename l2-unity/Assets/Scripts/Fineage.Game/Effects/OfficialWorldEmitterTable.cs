using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

/// <summary>Official map Emitter actors (location, rotation, DrawScale, prefab class, sounds).</summary>
public static class OfficialWorldEmitterTable
{
    public const string RelativePath = "Data/Meta/Effects/official-world-emitters.json.gz";

    [Serializable]
    public class FileData
    {
        public int emitterCount;
        public int prefabClasses;
        public Emitter[] emitters;
    }

    [Serializable]
    public class Emitter
    {
        public string map;
        public string name;
        public int tileX;
        public int tileY;
        public Vector3 location;
        public Vector3 rotation;
        public float drawScale;
        public Vector3 drawScale3D;
        public bool hasDrawScale3D;
        public string prefabClass;
        public string[] spawnSounds;
        public float soundRadius;
        public float soundVolume;
        public float visibleLimitMin;
        public float visibleLimitMax;
        public bool hasVisibleLimit;
        public string moveEvent;
        public bool hasGameEvent;

        public string Id => (map ?? "") + "/" + (name ?? "");

        public string ResourcesPath =>
            string.IsNullOrEmpty(prefabClass)
                ? null
                : "Data/Effects/world/" + prefabClass + "/" + prefabClass;

        public Vector3 UnityPosition => VectorUtils.ConvertPosToUnity(location);

        public Quaternion UnityRotation =>
            Quaternion.Euler(VectorUtils.ConvertRotToUnity(rotation));

        public Vector3 UnityScale
        {
            get
            {
                float s = drawScale != 0f ? drawScale : 1f;
                if (!hasDrawScale3D)
                    return Vector3.one * s;
                // UE actor-local X/Y/Z == Unity Z/X/Y (same swap as positions).
                return new Vector3(drawScale3D.y, drawScale3D.z, drawScale3D.x) * s;
            }
        }
    }

    public static Func<byte[]> BytesOverride;
    public static Action<string> ErrorLogged;

    static readonly Dictionary<string, List<Emitter>> ByMap =
        new Dictionary<string, List<Emitter>>(StringComparer.OrdinalIgnoreCase);
    static readonly List<Emitter> All = new List<Emitter>();
    static bool _loaded;

    public static IReadOnlyList<Emitter> AllEntries => All;
    public static int Count => All.Count;
    public static bool HasEmitters => Count > 0;

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
            Log("OfficialWorldEmitterTable: missing table path=\"" + pathForErrors + "\"");
            return;
        }

        string json;
        using (var raw = new MemoryStream(bytes, writable: false))
        using (Stream stream = IsGzip(bytes) ? (Stream)new GZipStream(raw, CompressionMode.Decompress) : raw)
        using (var reader = new StreamReader(stream))
            json = reader.ReadToEnd();

        FileData data = JsonUtility.FromJson<FileData>(json);
        if (data == null || data.emitters == null)
        {
            Log("OfficialWorldEmitterTable: parse error path=\"" + pathForErrors + "\"");
            return;
        }

        for (int i = 0; i < data.emitters.Length; i++)
        {
            Emitter e = data.emitters[i];
            if (e == null)
                continue;
            All.Add(e);
            string map = e.map ?? "";
            if (!ByMap.TryGetValue(map, out List<Emitter> list))
            {
                list = new List<Emitter>();
                ByMap[map] = list;
            }
            list.Add(e);
        }
    }

    public static IReadOnlyList<Emitter> ForMap(string map)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(map))
            return Array.Empty<Emitter>();
        return ByMap.TryGetValue(map, out List<Emitter> list) ? list : Array.Empty<Emitter>();
    }

    public static void CollectNearby(int tileX, int tileY, string sceneName, List<Emitter> dest)
    {
        EnsureLoaded();
        if (dest == null)
            return;
        dest.Clear();
        for (int dx = -1; dx <= 1; dx++)
        for (int dy = -1; dy <= 1; dy++)
        {
            string map = VectorUtils.FormatSwMapName(tileX + dx, tileY + dy);
            if (ByMap.TryGetValue(map, out List<Emitter> list))
                dest.AddRange(list);
        }

        if (!string.IsNullOrEmpty(sceneName) && ByMap.TryGetValue(sceneName, out List<Emitter> env))
        {
            for (int i = 0; i < env.Count; i++)
                dest.Add(env[i]);
        }
    }

    static void EnsureLoaded()
    {
        if (!_loaded)
            Load();
    }

    static bool IsGzip(byte[] bytes) =>
        bytes != null && bytes.Length >= 2 && bytes[0] == 0x1f && bytes[1] == 0x8b;

    static void Log(string message)
    {
        if (ErrorLogged != null)
            ErrorLogged(message);
        else
            Debug.LogError(message);
    }
}
