using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Original loop beds that the map export dropped: fire on lamps and braziers,
/// water on streams and falls. A source plays only while the listener is near it.
/// </summary>
public sealed class L2TownAmbience : MonoBehaviour
{
    const string Fire = "event:/AmbSound/AmbSound/fire_small_05";
    const string Fall = "event:/AmbSound/AmbSound/waterfall_big_02";
    const string Stream = "event:/AmbSound/AmbSound/stream_small_03";
    const float Hear = 28f;
    const float Wake = 32f;
    const float Sleep = 40f;

    struct Source
    {
        public Transform Transform;
        public string EventPath;
        public EventInstance Instance;
        public bool Playing;
    }

    readonly List<Source> _sources = new List<Source>();
    readonly HashSet<int> _seen = new HashSet<int>();
    readonly Dictionary<string, int> _scanned = new Dictionary<string, int>();
    float _next;

    void OnDisable()
    {
        for (int i = 0; i < _sources.Count; i++)
            Stop(i);
        _sources.Clear();
    }

    void LateUpdate()
    {
        if (Time.unscaledTime < _next)
            return;
        _next = Time.unscaledTime + 0.75f;

        Vector3 ear = Ear();
        ScanAround(ear);
        for (int i = 0; i < _sources.Count; i++)
            UpdateSource(i, ear);
    }

    static Vector3 Ear()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
            return player.transform.position;
        Camera cam = Camera.main;
        return cam != null ? cam.transform.position : Vector3.zero;
    }

    void ScanAround(Vector3 ear)
    {
        // Tile roots are named 21_19 / 18_14 / 23_12. Scan each once.
        int tx = Mathf.FloorToInt(ear.z * 52.5f / 32768f) + 20;
        int ty = Mathf.FloorToInt(ear.x * 52.5f / 32768f) + 18;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
                ScanTile((tx + dx).ToString("00") + "_" + (ty + dy).ToString("00"));
        }
    }

    void ScanTile(string tile)
    {
        int stamp = 0;
        Scene scene = SceneManager.GetSceneByName(tile);
        bool sceneLoaded = scene.IsValid() && scene.isLoaded;
        GameObject found = sceneLoaded ? null : GameObject.Find(tile);
        if (sceneLoaded)
            stamp = 1;
        else if (found != null)
            stamp = found.GetInstanceID();
        else
            return;

        if (_scanned.TryGetValue(tile, out int known) && known == stamp)
            return;

        if (sceneLoaded)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
                Scan(roots[i].transform);
        }
        else
            Scan(found.transform);

        _scanned[tile] = stamp;
    }

    void Scan(Transform root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null || !_seen.Add(t.GetInstanceID()))
                continue;
            string path = Kind(t.name);
            if (path == null)
                continue;
            _sources.Add(new Source { Transform = t, EventPath = path });
        }
    }

    static string Kind(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        string n = name.ToLowerInvariant();
        if (n.Contains("top") || n.Contains("beam") || n.Contains("grass") || n.Contains("lightmap"))
            return null;
        if (n.Contains("waterfall"))
            return Fall;
        if (n.Contains("stream") || n.Contains("river"))
            return Stream;
        if (n.Contains("fire") || n.Contains("lamp") || n.Contains("torch")
            || n.Contains("brazier") || n.Contains("furnace") || n.Contains("forge")
            || n.Contains("firepot") || n.Contains("dwarf_light"))
            return Fire;
        return null;
    }

    void UpdateSource(int index, Vector3 ear)
    {
        Source src = _sources[index];
        if (src.Transform == null)
        {
            Stop(index);
            return;
        }

        float dist = Vector3.Distance(ear, src.Transform.position);
        if (!src.Playing)
        {
            if (dist > Wake)
                return;
            Start(index);
            src = _sources[index];
        }
        else if (dist > Sleep)
        {
            Stop(index);
            return;
        }

        if (!src.Instance.isValid())
        {
            src.Playing = false;
            _sources[index] = src;
            return;
        }

        float t = Mathf.Clamp01(1f - Mathf.Max(0f, dist - 2f) / (Hear - 2f));
        src.Instance.setVolume(GameSettings.Sfx * t);
        src.Instance.set3DAttributes(RuntimeUtils.To3DAttributes(src.Transform.position));
        _sources[index] = src;
    }

    void Start(int index)
    {
        Source src = _sources[index];
        try
        {
            EventReference er = RuntimeManager.PathToEventReference(src.EventPath);
            if (er.IsNull)
                return;
            src.Instance = RuntimeManager.CreateInstance(er);
            src.Instance.set3DAttributes(RuntimeUtils.To3DAttributes(src.Transform.position));
            src.Instance.start();
            src.Playing = true;
            _sources[index] = src;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[TownAmbience] " + src.EventPath + " failed: " + ex.Message);
        }
    }

    void Stop(int index)
    {
        Source src = _sources[index];
        src.Playing = false;
        if (src.Instance.isValid())
        {
            src.Instance.stop(STOP_MODE.ALLOWFADEOUT);
            src.Instance.release();
            src.Instance.clearHandle();
        }

        _sources[index] = src;
    }
}
