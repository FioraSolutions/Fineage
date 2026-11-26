/// <summary>
/// Bounded 3D one-shot voices. A free slot is taken first. The same actor and UE2 sound slot
/// replaces its current voice. When every slot is busy, a strictly higher priority steals the quietest.
/// A lower priority is rejected. Rejection is not a pool error.
/// </summary>
public sealed class AudioEmitterPool
{
    public const int DefaultMaxWorldOneShots = 24;
    public const int DefaultMaxVoiceOneShots = 3;

    struct Voice
    {
        public bool Active;
        public float Priority;
        public int OwnerId;
        public int SoundSlot;
    }

    readonly Voice[] _world;
    readonly Voice[] _voice;
    int _activeWorld;
    int _activeVoice;

    public AudioEmitterPool(int maxWorld = DefaultMaxWorldOneShots, int maxVoice = DefaultMaxVoiceOneShots)
    {
        _world = new Voice[maxWorld > 0 ? maxWorld : 1];
        _voice = new Voice[maxVoice > 0 ? maxVoice : 1];
    }

    public int ActiveWorld => _activeWorld;
    public int ActiveVoice => _activeVoice;

    public bool TryAcquire(AudioCategory category, out int slot) =>
        TryClaim(category, 0f, 0, 0, out slot, out _, out _);

    public bool TryClaim(
        AudioCategory category,
        float priority,
        int ownerId,
        int soundSlot,
        out int slot,
        out bool replaced,
        out bool rejectedLowerPriority)
    {
        replaced = false;
        rejectedLowerPriority = false;
        slot = -1;
        bool voiceCategory = category == AudioCategory.Voice;
        Voice[] voices = voiceCategory ? _voice : _world;

        if (ownerId != 0)
        {
            for (int i = 0; i < voices.Length; i++)
            {
                if (!voices[i].Active || voices[i].OwnerId != ownerId || voices[i].SoundSlot != soundSlot)
                    continue;
                Voice replacedVoice = voices[i];
                replacedVoice.Priority = priority;
                voices[i] = replacedVoice;
                slot = i;
                replaced = true;
                return true;
            }
        }

        for (int i = 0; i < voices.Length; i++)
        {
            if (voices[i].Active)
                continue;
            Activate(voices, i, priority, ownerId, soundSlot, voiceCategory);
            slot = i;
            return true;
        }

        int lowest = 0;
        float lowestPriority = voices[0].Priority;
        for (int i = 1; i < voices.Length; i++)
        {
            if (voices[i].Priority < lowestPriority)
            {
                lowest = i;
                lowestPriority = voices[i].Priority;
            }
        }

        if (!(priority > lowestPriority))
        {
            rejectedLowerPriority = true;
            return false;
        }

        Voice stolen = voices[lowest];
        stolen.Active = true;
        stolen.Priority = priority;
        stolen.OwnerId = ownerId;
        stolen.SoundSlot = soundSlot;
        voices[lowest] = stolen;
        slot = lowest;
        replaced = true;
        return true;
    }

    public float PriorityOf(AudioCategory category, int slot)
    {
        Voice[] voices = category == AudioCategory.Voice ? _voice : _world;
        if (slot < 0 || slot >= voices.Length || !voices[slot].Active)
            return 0f;
        return voices[slot].Priority;
    }

    public void ReleaseIndex(AudioCategory category, int slot)
    {
        bool voiceCategory = category == AudioCategory.Voice;
        Voice[] voices = voiceCategory ? _voice : _world;
        if (slot < 0 || slot >= voices.Length || !voices[slot].Active)
            return;
        voices[slot] = default(Voice);
        if (voiceCategory)
        {
            if (_activeVoice > 0)
                _activeVoice--;
            return;
        }

        if (_activeWorld > 0)
            _activeWorld--;
    }

    public void Release(AudioCategory category)
    {
        bool voiceCategory = category == AudioCategory.Voice;
        Voice[] voices = voiceCategory ? _voice : _world;
        for (int i = voices.Length - 1; i >= 0; i--)
        {
            if (!voices[i].Active)
                continue;
            ReleaseIndex(category, i);
            return;
        }
    }

    public string FormatPoolExhaustedError(AudioCategory category) =>
        "AudioService: " + category + " emitter pool exhausted (max="
        + (category == AudioCategory.Voice ? _voice.Length : _world.Length) + ")";

    void Activate(Voice[] voices, int index, float priority, int ownerId, int soundSlot, bool voiceCategory)
    {
        Voice voice = voices[index];
        voice.Active = true;
        voice.Priority = priority;
        voice.OwnerId = ownerId;
        voice.SoundSlot = soundSlot;
        voices[index] = voice;
        if (voiceCategory)
            _activeVoice++;
        else
            _activeWorld++;
    }
}
