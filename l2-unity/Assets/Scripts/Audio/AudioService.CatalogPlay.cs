using UnityEngine;

public partial class AudioService
{
    public bool PlayCatalogSound(string soundId, Vector3? position = null, float volumeScale = 1f)
    {
        if (!AudioSoundCatalog.TryResolve(soundId, out AudioPlaySpec spec))
        {
            LogAudioError(AudioSoundCatalog.FormatUnknownIdError(soundId));
            return false;
        }

        return PlayFromSpec(spec, position, volumeScale);
    }

    public bool PlayFromSpec(AudioPlaySpec spec, Vector3? position, float volumeScale = 1f)
    {
        if (_disabled)
            return false;

        if (spec.Source == AudioSourceKind.FmodStudioEvent)
            return PlayFmodEvent(spec.Id, spec.Category, position);

        if (position.HasValue)
            return PlayClipAtWorld(spec.Id, position.Value, spec.Category, volumeScale, listener: false);

        return PlayClip2D(spec.Id, spec.Category, volumeScale);
    }
}