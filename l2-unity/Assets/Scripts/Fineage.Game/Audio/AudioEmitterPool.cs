using System.Collections.Generic;

/// <summary>Preallocated 3D one-shot slots — EditMode tests capacity rules without Unity audio I/O.</summary>
public sealed class AudioEmitterPool
{
    public const int DefaultMaxWorldOneShots = 24;
    public const int DefaultMaxVoiceOneShots = 3;

    readonly int _maxWorld;
    readonly int _maxVoice;
    int _activeWorld;
    int _activeVoice;

    public AudioEmitterPool(int maxWorld = DefaultMaxWorldOneShots, int maxVoice = DefaultMaxVoiceOneShots)
    {
        _maxWorld = maxWorld;
        _maxVoice = maxVoice;
    }

    public int ActiveWorld => _activeWorld;
    public int ActiveVoice => _activeVoice;

    public bool TryAcquire(AudioCategory category, out int slot)
    {
        slot = -1;
        if (category == AudioCategory.Voice)
        {
            if (_activeVoice >= _maxVoice)
                return false;
            slot = _activeVoice;
            _activeVoice++;
            return true;
        }

        if (_activeWorld >= _maxWorld)
            return false;
        slot = _activeWorld;
        _activeWorld++;
        return true;
    }

    public void Release(AudioCategory category)
    {
        if (category == AudioCategory.Voice)
        {
            if (_activeVoice > 0)
                _activeVoice--;
            return;
        }

        if (_activeWorld > 0)
            _activeWorld--;
    }

    public string FormatPoolExhaustedError(AudioCategory category) =>
        "AudioService: " + category + " emitter pool exhausted (max="
        + (category == AudioCategory.Voice ? _maxVoice : _maxWorld) + ")";
}