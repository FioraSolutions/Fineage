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
    const int TileReach = 2;

    sealed class Voice
    {
        public string Id;
        public OfficialAmbientSoundTable.Entry Entry;
        public AudioSource Source;
        public AudioClip Clip;
        public float NextOneShotAt;
        public float Period;
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
            UpdateVoice(entry, ear, _candidates[i].dist);
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
            float radiusUu = Ue2SoundAudibility.ResolveActorSoundRadiusUu(entry.RadiusUu);
            float dist = Vector3.Distance(ear, entry.UnityPosition);
            if (Ue2SoundAudibility.IsAudible(dist, radiusUu))
                _candidates.Add((entry, dist));
        }
    }

    void UpdateVoice(OfficialAmbientSoundTable.Entry entry, Vector3 ear, float distance)
    {
        if (!_voices.TryGetValue(entry.Id, out Voice voice))
        {
            if (!TryCreateVoice(entry, distance, out voice))
                return;
            _voices[entry.Id] = voice;
        }

        voice.Source.transform.position = entry.UnityPosition;
        if (voice.Looping)
            return;

        if (voice.Period <= 0f || Time.unscaledTime < voice.NextOneShotAt)
            return;
        if (!RollChance(entry.Random))
        {
            voice.NextOneShotAt = Time.unscaledTime + voice.Period;
            return;
        }

        if (!voice.Source.isPlaying)
        {
            if (!AudioListenerLifecycle.TryPrepare())
            {
                TraceAmbient(entry, distance, "refuse", "no-listener");
                return;
            }

            voice.Source.Play();
            TraceAmbient(entry, distance, "play", RadiusReason(entry));
        }
        voice.NextOneShotAt = Time.unscaledTime + voice.Period;
    }

    bool TryCreateVoice(OfficialAmbientSoundTable.Entry entry, float distance, out Voice voice)
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
        AudioService.ApplyUe2LoopFalloff(source, Ue2SoundAudibility.ResolveActorSoundRadiusUu(entry.RadiusUu));
        source.transform.position = entry.UnityPosition;
        if (source.loop && clip.length > 0f && entry.StartTime != 0f)
        {
            float length = clip.length;
            float phase = entry.StartTime % length;
            if (phase < 0f)
                phase += length;
            source.time = phase;
        }
        bool oneShot = !source.loop;
        float period = clip.length;
        if (oneShot && period <= 0.01f)
        {
            ReleaseSource(source);
            LogOnce(
                "period:" + entry.Id,
                "OfficialAmbientSoundPlayback: one-shot has no wave duration map=\""
                + entry.Map + "\" name=\"" + entry.Name + "\" sound=\"" + entry.Sound + "\".");
            return false;
        }

        if (!AudioListenerLifecycle.TryPrepare())
        {
            ReleaseSource(source);
            TraceAmbient(entry, distance, "refuse", "no-listener");
            return false;
        }

        if (!oneShot)
        {
            source.Play();
            TraceAmbient(entry, distance, "play", RadiusReason(entry));
        }

        voice = new Voice
        {
            Id = entry.Id,
            Entry = entry,
            Source = source,
            Clip = clip,
            Looping = !oneShot,
            Period = oneShot ? period : 0f,
            // AmbientSoundStartTime is a per-actor timestamp. There is no shared
            // retrigger interval on the actor, so the wave's own length is the cycle
            // and StartTime only phases it.
            NextOneShotAt = Time.unscaledTime + (oneShot ? Phase(entry.StartTime, period) : 0f)
        };
        return true;
    }

    static float Phase(float startTime, float period)
    {
        if (period <= 0.01f || startTime == 0f)
            return 0f;
        float phase = startTime % period;
        if (phase < 0f)
            phase += period;
        return phase;
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

    static string RadiusReason(OfficialAmbientSoundTable.Entry entry) =>
        Ue2SoundAudibility.HasRadius(entry.RadiusUu) ? "audible" : "actor-default";

    static void TraceAmbient(OfficialAmbientSoundTable.Entry entry, float distance, string action, string reason)
    {
        float radiusUu = Ue2SoundAudibility.ResolveActorSoundRadiusUu(entry.RadiusUu);
        float scale = OfficialAmbientSoundUnits.VolumeToScale(entry.Volume);
        float applied = scale * Ue2SoundAudibility.Attenuation(distance, radiusUu);
        AudioPlayTrace.Write(
            action,
            Time.unscaledTime,
            entry.Sound,
            "Ambient",
            entry.Name,
            0,
            distance,
            radiusUu,
            applied,
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

    void LogOnce(string key, string message)
    {
        if (!_loggedMissing.Add(key))
            return;
        Debug.LogError(message);
    }
}
