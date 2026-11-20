using UnityEngine;

public partial class AudioService
{
    public void PlaceCasterChannels(Vector3 worldPos, bool listener)
    {
        PlaceChannel(_castChannel, worldPos, listener);
        PlaceChannel(_voiceChannel, worldPos, listener);
    }

    static void PlaceChannel(AudioSource src, Vector3 worldPos, bool listener)
    {
        if (src == null)
            return;
        if (listener)
        {
            src.spatialBlend = 0f;
            src.transform.localPosition = Vector3.zero;
            return;
        }

        ApplyWorldFalloff(src);
        src.transform.position = worldPos;
    }

    public void PlayCastClipByClip(AudioClip clip, float volumeScale = 0.85f)
    {
        if (_disabled || clip == null)
            return;
        if (_castChannel == null)
        {
            LogAudioError("AudioService: cast channel missing.");
            return;
        }

        _castChannel.Stop();
        _castChannel.clip = clip;
        _castChannel.volume = CategoryVolume(AudioCategory.Sfx) * volumeScale;
        _castChannel.loop = false;
        _castChannel.Play();
    }

    public void PlayCastClip(string resourcesPath, float volumeScale = 0.85f)
    {
        if (_disabled)
            return;

        if (!TryLoadClip(resourcesPath, out AudioClip clip, out string error))
        {
            if (!string.IsNullOrEmpty(error))
                LogAudioError(error);
            return;
        }

        if (_castChannel == null)
        {
            LogAudioError("AudioService: cast channel missing.");
            return;
        }

        _castChannel.Stop();
        _castChannel.clip = clip;
        _castChannel.volume = CategoryVolume(AudioCategory.Sfx) * volumeScale;
        _castChannel.loop = false;
        _castChannel.Play();
    }

    public void StopCast()
    {
        if (_castChannel != null && _castChannel.isPlaying)
            _castChannel.Stop();
    }

    public void PlayVoiceClip(string resourcesPath, float volumeScale = 1f)
    {
        if (_disabled)
            return;

        if (!TryLoadClip(resourcesPath, out AudioClip clip, out string error))
        {
            if (!string.IsNullOrEmpty(error))
                LogAudioError(error);
            return;
        }

        PlayVoiceClipInternal(clip, volumeScale, resourcesPath);
    }

    public void PlayVoiceClipInternal(AudioClip clip, float volumeScale, string idForLog)
    {
        if (clip == null)
        {
            LogAudioError(AudioClipCache.FormatMissingClipError(idForLog));
            return;
        }

        if (_voiceChannel == null)
        {
            LogAudioError("AudioService: voice channel missing.");
            return;
        }

        _voiceChannel.Stop();
        _voiceChannel.clip = clip;
        _voiceChannel.volume = CategoryVolume(AudioCategory.Voice) * volumeScale;
        _voiceChannel.loop = false;
        _voiceChannel.Play();
    }

    public void StopVoice()
    {
        if (_voiceChannel != null && _voiceChannel.isPlaying)
            _voiceChannel.Stop();
    }

    public bool PlayHitImpact(bool criticalHit, Vector3 position)
    {
        if (criticalHit)
        {
            if (PlayFmodEvent(AudioPathResolver.FmodSkillSound("critical_hit"), AudioCategory.Sfx, position))
                return true;
            return PlayResourcesPath(AudioPathResolver.ResourcesSkillClip("critical_hit"), 0.75f)
                || PlayResourcesPath(AudioPathResolver.ResourcesSkillClip("critical_hit_stereo"), 0.75f)
                || PlayResourcesPath(AudioPathResolver.ResourcesSkillClip("critical_hit_02"), 0.75f);
        }

        if (PlayFmodEvent(AudioPathResolver.FmodItemSound("armor_hit_underwear"), AudioCategory.Sfx, position))
            return true;

        int idx = (Random.Range(0, 4) % 4) + 1;
        string armor = AudioPathResolver.ResourcesItemSoundRoot + "armor_underwear_" + idx;
        if (PlayClip2D(armor, AudioCategory.Sfx, 0.75f))
            return true;
        return PlayClip2D(AudioPathResolver.ResourcesItemSoundRoot + "armor_underwear_1", AudioCategory.Sfx, 0.75f);
    }
}