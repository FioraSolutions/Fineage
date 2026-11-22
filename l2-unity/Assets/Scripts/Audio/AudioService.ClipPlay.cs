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

        bus.PlayOneShot(clip, CategoryVolume(category) * volumeScale);
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
        int soundSlot = 0)
    {
        if (_disabled || clip == null)
            return false;

        if (atListener)
        {
            PlayLoadedClip2D(clip, category, volumeScale, idForLog);
            return true;
        }

        if (!Ue2SoundAudibility.HasRadius(innerRadiusUu))
        {
            LogAudioErrorOnce("radius:" + (idForLog ?? ""), Ue2SoundAudibility.FormatMissingRadius(idForLog));
            return true;
        }

        if (!TryDistanceToListener(worldPos, out float distance))
        {
            LogAudioErrorOnce("listener", "AudioService: no listener for 3D sound \"" + (idForLog ?? "") + "\".");
            return true;
        }

        if (!Ue2SoundAudibility.IsAudible(distance, innerRadiusUu))
            return true;

        float priority = Ue2SoundAudibility.Priority(volumeScale, distance, innerRadiusUu);
        if (!_pool.TryClaim(category, priority, ownerId, soundSlot, out int slot, out bool replaced, out _))
            return true;

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
        string idForLog = null)
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
            clip, worldPos, category, volumeScale, atListener, id, innerRadiusUu, ownerId, soundSlot);
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