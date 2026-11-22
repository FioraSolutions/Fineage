using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plays official AmbientSoundObject rows around the listener. Replaces L2TownAmbience
/// name-matching and baked FMOD emitters once the official table is present.
/// </summary>
public sealed class OfficialAmbientSoundPlayback : MonoBehaviour
{
    const float TickSeconds = 0.35f;
    const int MaxVoices = 64;
    const float RandomRetriggerDelay = 3f;
    const int TileReach = 2;

    sealed class Voice
    {
        public string Id;
        public OfficialAmbientSoundTable.Entry Entry;
        public AudioSource Source;
        public AudioClip Clip;
        public float NextOneShotAt;
        public bool Looping;
    }

    readonly Dictionary<string, Voice> _voices = new Dictionary<string, Voice>();
    readonly List<string> _stop = new List<string>();
    readonly List<(OfficialAmbientSoundTable.Entry entry, float dist)> _candidates =
        new List<(OfficialAmbientSoundTable.Entry, float)>();
    readonly HashSet<string> _loggedMissing = new HashSet<string>();
    readonly Stack<AudioSource> _pool = new Stack<AudioSource>();
    float _next;

    void OnEnable()
    {
        OfficialWaveTable.Load();
        OfficialAmbientSoundTable.Load();
    }

    void OnDisable()
    {
        foreach (var pair in _voices)
            Release(pair.Value);
        _voices.Clear();
    }

    void LateUpdate()
    {
        if (Time.unscaledTime < _next)
            return;
        _next = Time.unscaledTime + TickSeconds;

        if (!OfficialAmbientSoundTable.HasActors)
            return;
        if (!AudioWorldGate.TryGetListenerPosition(out Vector3 ear))
            return;

        CollectCandidates(ear);
        if (_candidates.Count > MaxVoices)
        {
            _candidates.Sort((a, b) => a.dist.CompareTo(b.dist));
            _candidates.RemoveRange(MaxVoices, _candidates.Count - MaxVoices);
        }

        var keep = new HashSet<string>();
        for (int i = 0; i < _candidates.Count; i++)
        {
            OfficialAmbientSoundTable.Entry entry = _candidates[i].entry;
            keep.Add(entry.Id);
            UpdateVoice(entry, ear);
        }

        _stop.Clear();
        foreach (var pair in _voices)
        {
            if (!keep.Contains(pair.Key))
                _stop.Add(pair.Key);
        }
        for (int i = 0; i < _stop.Count; i++)
        {
            Release(_voices[_stop[i]]);
            _voices.Remove(_stop[i]);
        }
    }

    void CollectCandidates(Vector3 ear)
    {
        _candidates.Clear();
        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(ear);
        VectorUtils.GetSwMapTile(l2, out int tileX, out int tileY);

        for (int dx = -TileReach; dx <= TileReach; dx++)
        {
            for (int dy = -TileReach; dy <= TileReach; dy++)
                ConsiderMap(VectorUtils.FormatSwMapName(tileX + dx, tileY + dy), ear);
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.IsValid() && !string.IsNullOrEmpty(scene.name))
            ConsiderMap(scene.name, ear);
    }

    void ConsiderMap(string map, Vector3 ear)
    {
        IReadOnlyList<OfficialAmbientSoundTable.Entry> rows = OfficialAmbientSoundTable.ForMap(map);
        for (int i = 0; i < rows.Count; i++)
        {
            OfficialAmbientSoundTable.Entry entry = rows[i];
            if (!IsTypeActive(entry.Type))
                continue;
            float radius = HearRadiusUnity(entry);
            float dist = HorizontalDistance(ear, entry.UnityPosition);
            if (dist <= radius)
                _candidates.Add((entry, dist));
        }
    }

    void UpdateVoice(OfficialAmbientSoundTable.Entry entry, Vector3 ear)
    {
        if (!_voices.TryGetValue(entry.Id, out Voice voice))
        {
            if (!TryCreateVoice(entry, out voice))
                return;
            _voices[entry.Id] = voice;
        }

        voice.Source.transform.position = entry.UnityPosition;
        if (voice.Looping)
            return;

        if (Time.unscaledTime < voice.NextOneShotAt)
            return;
        if (!RollChance(entry.Random))
        {
            voice.NextOneShotAt = Time.unscaledTime + RandomRetriggerDelay;
            return;
        }

        if (!voice.Source.isPlaying)
        {
            if (!AudioListenerLifecycle.TryPrepare())
            {
                TraceAmbient(entry, "refuse", "no-listener");
                return;
            }

            voice.Source.Play();
            TraceAmbient(entry, "play", RadiusReason(entry));
        }
        voice.NextOneShotAt = Time.unscaledTime + RandomRetriggerDelay;
    }

    bool TryCreateVoice(OfficialAmbientSoundTable.Entry entry, out Voice voice)
    {
        voice = null;
        if (string.IsNullOrEmpty(entry.Sound))
        {
            LogOnce("empty:" + entry.Id, OfficialWaveTable.FormatMissingWaveError(
                null, entry.Sound, "ambient actor map=\"" + entry.Map + "\" name=\"" + entry.Name + "\""));
            return false;
        }

        string resourcesPath;
        string error;
        if (entry.Sound.IndexOf('.') >= 0)
        {
            if (!OfficialWaveTable.TryResolveDatToken(entry.Sound, out resourcesPath, out error))
            {
                LogOnce("wave:" + entry.Sound, error ?? OfficialWaveTable.FormatMissingWaveError(null, entry.Sound, "ambient sound"));
                return false;
            }
        }
        else if (!OfficialWaveTable.TryResolve(entry.Sound, out OfficialWaveTable.Entry wave, out error))
        {
            LogOnce("wave:" + entry.Sound, error ?? OfficialWaveTable.FormatMissingWaveError(null, entry.Sound, "ambient sound"));
            return false;
        }
        else
            resourcesPath = wave.ResourcesPath;

        AudioService service = AudioPacketServices.Service;
        if (service == null)
            return false;
        if (!service.TryLoadClip(resourcesPath, out AudioClip clip, out string loadError))
        {
            LogOnce("clip:" + resourcesPath, loadError ?? AudioClipCache.FormatMissingClipError(resourcesPath));
            return false;
        }

        AudioSource source = AcquireSource();
        source.clip = clip;
        source.spatialBlend = 1f;
        source.dopplerLevel = 0f;
        source.loop = entry.Random == 0;
        source.pitch = OfficialAmbientSoundUnits.PitchToMultiplier(entry.Pitch);
        source.volume = OfficialAmbientSoundUnits.VolumeToScale(entry.Volume);
        float hear = HearRadiusUnity(entry);
        AudioService.ApplyWorldFalloff(source, hear);
        source.minDistance = Mathf.Min(AudioWorldConstants.HearMinMeters, Mathf.Max(0.01f, hear * 0.05f));
        source.transform.position = entry.UnityPosition;
        if (source.loop && clip.length > 0f && entry.StartTime != 0f)
        {
            float length = clip.length;
            float phase = entry.StartTime % length;
            if (phase < 0f)
                phase += length;
            source.time = phase;
        }
        if (!AudioListenerLifecycle.TryPrepare())
        {
            ReleaseSource(source);
            TraceAmbient(entry, "refuse", "no-listener");
            return false;
        }

        source.Play();
        TraceAmbient(entry, "play", RadiusReason(entry));

        voice = new Voice
        {
            Id = entry.Id,
            Entry = entry,
            Source = source,
            Clip = clip,
            Looping = source.loop,
            NextOneShotAt = Time.unscaledTime + RandomRetriggerDelay
        };
        return true;
    }

    AudioSource AcquireSource()
    {
        while (_pool.Count > 0)
        {
            AudioSource reused = _pool.Pop();
            if (reused != null)
            {
                reused.gameObject.SetActive(true);
                return reused;
            }
        }

        var go = new GameObject("OfficialAmbientVoice");
        go.transform.SetParent(transform, false);
        var source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    void Release(Voice voice)
    {
        if (voice == null || voice.Source == null)
            return;
        ReleaseSource(voice.Source);
    }

    void ReleaseSource(AudioSource source)
    {
        if (source == null)
            return;
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        _pool.Push(source);
    }

    static float HearRadiusUnity(OfficialAmbientSoundTable.Entry entry) =>
        OfficialAmbientSoundUnits.RadiusToUnity(Ue2SoundAudibility.ResolveActorSoundRadiusUu(entry.RadiusUu));

    static string RadiusReason(OfficialAmbientSoundTable.Entry entry) =>
        Ue2SoundAudibility.HasRadius(entry.RadiusUu) ? "audible" : "actor-default";

    static void TraceAmbient(OfficialAmbientSoundTable.Entry entry, string action, string reason)
    {
        float radiusUu = Ue2SoundAudibility.ResolveActorSoundRadiusUu(entry.RadiusUu);
        AudioPlayTrace.Write(
            action,
            Time.unscaledTime,
            entry.Sound,
            "Ambient",
            entry.Name,
            0,
            0f,
            radiusUu,
            entry.Volume,
            Ue2SoundAudibility.SlotAmbient,
            reason);
    }

    static bool IsTypeActive(AmbientSoundType type)
    {
        if (type == AmbientSoundType.AST1_Always
            || type == AmbientSoundType.AST1_Water
            || type == AmbientSoundType.AST1_RangeTime)
            return true;

        WorldClock clock = WorldClockPacketServices.Clock;
        bool night = clock != null && clock.IsNightTime();
        if (type == AmbientSoundType.AST1_Day)
            return !night;
        if (type == AmbientSoundType.AST1_Night)
            return night;
        return true;
    }

    static bool RollChance(int ambientRandom)
    {
        if (ambientRandom <= 0)
            return true;
        return Random.Range(1, 101) <= ambientRandom;
    }

    static float HorizontalDistance(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    void LogOnce(string key, string message)
    {
        if (!_loggedMissing.Add(key))
            return;
        Debug.LogError(message);
    }
}
