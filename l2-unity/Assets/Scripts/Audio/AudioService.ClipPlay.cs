using UnityEngine;

public partial class AudioService
{
    public bool TryLoadClip(string resourcesPath, out AudioClip clip, out string error) =>
        AudioClipCache.TryLoad(resourcesPath, Time.realtimeSinceStartup, out clip, out error);

    public bool PlayResourcesPath(string resourcesPath, float volumeScale = 1f)
    {
        AudioPlaySpec spec = AudioPathResolver.SpecForResourcesPath(resourcesPath);
        if (spec.Source != AudioSourceKind.ResourcesClip)
            return false;
        return PlayClip2D(resourcesPath, spec.Category, volumeScale);
    }

    public bool PlayClip2D(string resourcesPath, AudioCategory category, float volumeScale = 1f)
    {
        if (_disabled)
            return false;

        if (!TryLoadClip(resourcesPath, out AudioClip clip, out string error))
        {
            if (!string.IsNullOrEmpty(error))
                LogAudioError(error);
            return false;
        }

        return PlayLoadedClip2D(clip, category, volumeScale, resourcesPath);
    }

    public bool PlayLoadedClip2D(AudioClip clip, AudioCategory category, float volumeScale, string idForLog) =>
        PlayCachedClip2D(clip, category, volumeScale, idForLog);

    bool PlayCachedClip2D(AudioClip clip, AudioCategory category, float volumeScale, string idForLog)
    {
        if (clip == null)
        {
            LogAudioError(AudioClipCache.FormatMissingClipError(idForLog));
            return false;
        }

        AudioSource bus = category == AudioCategory.Voice ? _voiceChannel : _oneShot2D;
        if (bus == null)
        {
            LogAudioError("AudioService: 2D channel missing for category=" + category);
            return false;
        }

        if (!AudioListenerLifecycle.TryPrepare())
        {
            Trace("refuse", idForLog, "Clip2D", null, 0, 0f, 0f, volumeScale, 0, "no-listener");
            return false;
        }

        bus.PlayOneShot(clip, CategoryVolume(category) * volumeScale);
        Trace("play", idForLog, "Clip2D", null, 0, 0f, 0f, volumeScale, 0, "ui");
        return true;
    }

    public bool PlayClipAt(
        AudioClip clip,
        Vector3 worldPos,
        AudioCategory category,
        float volumeScale,
        bool atListener,
        string idForLog,
        float innerRadiusUu = float.NaN,
        int ownerId = 0,
        int soundSlot = 0,
        string sourceSystem = null,
        string ownerName = null)
    {
        if (_disabled || clip == null)
            return false;

        string source = string.IsNullOrEmpty(sourceSystem) ? "WorldClip" : sourceSystem;
        if (atListener)
            return PlayLoadedClip2D(clip, category, volumeScale, idForLog);

        if (!Ue2SoundAudibility.HasRadius(innerRadiusUu))
        {
            LogAudioErrorOnce("radius:" + (idForLog ?? ""), Ue2SoundAudibility.FormatMissingRadius(idForLog));
            Trace("refuse", idForLog, source, ownerName, ownerId, 0f, innerRadiusUu, volumeScale, soundSlot, "missing-radius");
            return true;
        }

        if (!TryDistanceToListener(worldPos, out float distance))
        {
            LogAudioErrorOnce("listener", "AudioService: no listener for 3D sound \"" + (idForLog ?? "") + "\".");
            Trace("refuse", idForLog, source, ownerName, ownerId, 0f, innerRadiusUu, volumeScale, soundSlot, "no-listener");
            return true;
        }

        if (!Ue2SoundAudibility.IsAudible(distance, innerRadiusUu))
        {
            Trace("refuse", idForLog, source, ownerName, ownerId, distance, innerRadiusUu, volumeScale, soundSlot, "inaudible");
            return true;
        }

        float priority = Ue2SoundAudibility.Priority(volumeScale, distance, innerRadiusUu);
        if (!_pool.TryClaim(category, priority, ownerId, soundSlot, out int slot, out bool replaced, out _))
        {
            Trace("refuse", idForLog, source, ownerName, ownerId, distance, innerRadiusUu, volumeScale, soundSlot, "rejected");
            return true;
        }

        if (!AudioListenerLifecycle.TryPrepare())
        {
            _pool.ReleaseIndex(category, slot);
            Trace("refuse", idForLog, source, ownerName, ownerId, distance, innerRadiusUu, volumeScale, soundSlot, "no-listener");
            return true;
        }

        WorldEmitter emitter = replaced ? FindEmitter(category, slot) : null;
        if (emitter == null)
            emitter = AcquireWorldEmitter();
        if (emitter.Source != null && emitter.Source.isPlaying)
            emitter.Source.Stop();

        float attenuation = Ue2SoundAudibility.Attenuation(distance, innerRadiusUu);
        emitter.Category = category;
        emitter.VoiceIndex = slot;
        emitter.Go.SetActive(true);
        emitter.Go.transform.position = worldPos;
        ApplyAudibleFalloff(emitter.Source, innerRadiusUu);
        emitter.Source.PlayOneShot(clip, CategoryVolume(category) * volumeScale * attenuation);
        emitter.ReleaseAt = Time.unscaledTime + clip.length + 0.15f;
        Trace(replaced ? "steal" : "play", idForLog, source, ownerName, ownerId, distance, innerRadiusUu, volumeScale, slot, replaced ? "stolen" : "audible");
        return true;
    }

    public bool PlayClipAtWorld(
        string resourcesPath,
        Vector3 worldPos,
        AudioCategory category,
        float volumeScale,
        bool atListener,
        float innerRadiusUu = float.NaN,
        int ownerId = 0,
        int soundSlot = 0,
        string idForLog = null,
        string sourceSystem = null,
        string ownerName = null)
    {
        if (_disabled)
            return false;

        if (!TryLoadClip(resourcesPath, out AudioClip clip, out string error))
        {
            if (!string.IsNullOrEmpty(error))
                LogAudioError(error);
            return false;
        }

        string id = string.IsNullOrEmpty(idForLog) ? resourcesPath : idForLog;
        return PlayClipAt(
            clip, worldPos, category, volumeScale, atListener, id, innerRadiusUu, ownerId, soundSlot,
            sourceSystem, ownerName);
    }

    void Trace(
        string action,
        string clipId,
        string source,
        string ownerName,
        int ownerId,
        float distance,
        float radiusUu,
        float volume,
        int slot,
        string reason)
    {
        AudioPlayTrace.Write(
            action, Time.unscaledTime, clipId, source, ownerName, ownerId,
            distance, radiusUu, volume, slot, reason);
    }

    bool TryDistanceToListener(Vector3 worldPos, out float distance)
    {
        if (!AudioWorldGate.TryGetListenerPosition(out Vector3 listener))
        {
            distance = 0f;
            return false;
        }

        distance = Vector3.Distance(worldPos, listener);
        return true;
    }

    WorldEmitter FindEmitter(AudioCategory category, int voiceIndex)
    {
        for (int i = 0; i < _worldEmitters.Count; i++)
        {
            WorldEmitter emitter = _worldEmitters[i];
            if (emitter.VoiceIndex == voiceIndex && emitter.Category == category)
                return emitter;
        }

        return null;
    }

    WorldEmitter AcquireWorldEmitter()
    {
        WorldEmitter emitter;
        for (int i = 0; i < _worldEmitters.Count; i++)
        {
            WorldEmitter candidate = _worldEmitters[i];
            if (candidate.Source == null || candidate.VoiceIndex >= 0)
                continue;
            if (candidate.Source.isPlaying)
                continue;
            candidate.Go.SetActive(true);
            return candidate;
        }

        var go = new GameObject("PooledWorldSfx");
        go.transform.SetParent(_clipRoot, true);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        emitter = new WorldEmitter { Go = go, Source = src };
        _worldEmitters.Add(emitter);
        return emitter;
    }

    void TickWorldEmitters()
    {
        float now = Time.unscaledTime;
        for (int i = 0; i < _worldEmitters.Count; i++)
        {
            WorldEmitter e = _worldEmitters[i];
            if (e.Source == null)
                continue;
            if (e.ReleaseAt > 0f && now >= e.ReleaseAt)
            {
                e.Source.Stop();
                e.Go.SetActive(false);
                e.ReleaseAt = 0f;
                if (e.VoiceIndex >= 0)
                {
                    _pool.ReleaseIndex(e.Category, e.VoiceIndex);
                    e.VoiceIndex = -1;
                }
            }
        }
    }

    static void ApplyAudibleFalloff(AudioSource src, float innerRadiusUu)
    {
        float outer = Ue2SoundAudibility.OuterRadiusMeters(innerRadiusUu);
        src.spatialBlend = 1f;
        src.dopplerLevel = 0f;
        src.rolloffMode = AudioRolloffMode.Linear;
        // Gain already carries the UE2 curve. Keep Unity flat inside the cutoff so it only pans.
        src.minDistance = outer;
        src.maxDistance = outer + 0.01f;
    }

    /// <summary>
    /// Looping actor sound. Same inner plateau and Radius*100 cutoff as one-shots.
    /// The custom curve is <see cref="Ue2SoundAudibility.Attenuation"/> sampled across that span.
    /// </summary>
    internal static void ApplyUe2LoopFalloff(AudioSource src, float innerRadiusUu)
    {
        float inner = Ue2SoundAudibility.InnerRadiusMeters(innerRadiusUu);
        float outer = Ue2SoundAudibility.OuterRadiusMeters(innerRadiusUu);
        if (inner < 0.01f)
            inner = 0.01f;
        if (outer <= inner)
            outer = inner + 0.01f;
        src.spatialBlend = 1f;
        src.dopplerLevel = 0f;
        src.rolloffMode = AudioRolloffMode.Custom;
        src.minDistance = inner;
        src.maxDistance = outer;
        src.SetCustomCurve(AudioSourceCurveType.CustomRolloff, BuildUe2Rolloff(innerRadiusUu, inner, outer));
    }

    static AnimationCurve BuildUe2Rolloff(float innerRadiusUu, float inner, float outer)
    {
        const int steps = 8;
        var keys = new Keyframe[steps + 2];
        keys[0] = new Keyframe(0f, 1f, 0f, 0f);
        for (int i = 0; i <= steps; i++)
        {
            float distance = inner + (outer - inner) * (i / (float)steps);
            float gain = Ue2SoundAudibility.Attenuation(distance, innerRadiusUu);
            float inTan = 0f;
            float outTan = 0f;
            keys[i + 1] = new Keyframe(distance, gain, inTan, outTan);
        }

        for (int i = 0; i < keys.Length - 1; i++)
        {
            float dt = keys[i + 1].time - keys[i].time;
            float slope = dt > 0.0001f ? (keys[i + 1].value - keys[i].value) / dt : 0f;
            keys[i].outTangent = slope;
            keys[i + 1].inTangent = slope;
        }

        return new AnimationCurve(keys);
    }

    internal static void ApplyWorldFalloff(AudioSource src, float maxDistanceMeters = -1f)
    {
        src.spatialBlend = 1f;
        src.dopplerLevel = 0f;
        src.rolloffMode = AudioRolloffMode.Linear;
        float maxDistance = maxDistanceMeters > 0f ? maxDistanceMeters : WorldHearMaxMeters;
        src.minDistance = WorldHearMinMeters;
        src.maxDistance = maxDistance;
        if (src.minDistance >= src.maxDistance)
            src.minDistance = Mathf.Max(0.01f, src.maxDistance * 0.1f);
    }

    public void PlayDelayed(float delaySeconds, System.Action action)
    {
        if (action == null)
            return;
        if (delaySeconds <= 0f)
        {
            action();
            return;
        }

        var runner = GetComponent<AudioServiceRunner>();
        if (runner == null)
            runner = gameObject.AddComponent<AudioServiceRunner>();
        runner.StartCoroutine(DelayThen(delaySeconds, action));
    }

    static System.Collections.IEnumerator DelayThen(float delay, System.Action action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action();
    }
}