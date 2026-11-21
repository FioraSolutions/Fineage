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

    public void PlayClipAt(
        AudioClip clip,
        Vector3 worldPos,
        AudioCategory category,
        float volumeScale,
        bool atListener,
        string idForLog)
    {
        if (_disabled || clip == null)
            return;

        if (atListener)
        {
            PlayLoadedClip2D(clip, category, volumeScale, idForLog);
            return;
        }

        int slot;
        if (!_pool.TryAcquire(category, out slot))
        {
            LogAudioError(_pool.FormatPoolExhaustedError(category));
            return;
        }

        WorldEmitter emitter = AcquireWorldEmitter();
        emitter.Category = category;
        emitter.Go.transform.position = worldPos;
        ApplyWorldFalloff(emitter.Source);
        emitter.Source.PlayOneShot(clip, CategoryVolume(category) * volumeScale);
        emitter.ReleaseAt = Time.unscaledTime + clip.length + 0.15f;
    }

    public bool PlayClipAtWorld(
        string resourcesPath,
        Vector3 worldPos,
        AudioCategory category,
        float volumeScale,
        bool atListener)
    {
        if (_disabled)
            return false;

        if (!TryLoadClip(resourcesPath, out AudioClip clip, out string error))
        {
            if (!string.IsNullOrEmpty(error))
                LogAudioError(error);
            return false;
        }

        PlayClipAt(clip, worldPos, category, volumeScale, atListener, resourcesPath);
        return true;
    }

    WorldEmitter AcquireWorldEmitter()
    {
        WorldEmitter emitter;
        for (int i = 0; i < _worldEmitters.Count; i++)
        {
            if (_worldEmitters[i].Source != null && !_worldEmitters[i].Source.isPlaying)
            {
                emitter = _worldEmitters[i];
                emitter.Go.SetActive(true);
                return emitter;
            }
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
                _pool.Release(e.Category);
            }
        }
    }

    internal static void ApplyWorldFalloff(AudioSource src)
    {
        src.spatialBlend = 1f;
        src.dopplerLevel = 0f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = WorldHearMinMeters;
        src.maxDistance = WorldHearMaxMeters;
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