using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
/// <summary>
/// Single entry point for game audio. FMOD Studio is the playback engine for banked events;
/// Interlude skills/voices use cached Resources clips on dedicated channels inside this service.
/// </summary>
public partial class AudioService : MonoBehaviour
{
    public const float WorldHearMinMeters = AudioWorldConstants.HearMinMeters;
    public const float WorldHearMaxMeters = AudioWorldConstants.HearMaxMeters;

    [Header("Volume (inspector defaults; multiplied with GameSettings)")]
    [Range(0, 1)] [SerializeField] float _masterVolume = 1f;
    [Range(0, 1)] [SerializeField] float _musicVolume = 1f;
    [Range(0, 1)] [SerializeField] float _ambientVolume = 1f;
    [Range(0, 1)] [SerializeField] float _sfxVolume = 1f;
    [Range(0, 1)] [SerializeField] float _uiVolume = 1f;
    [Range(0, 1)] [SerializeField] float _voiceVolume = 1f;
    [SerializeField] bool _muteWhenNotFocused = true;
    [SerializeField] bool _disabled;

    static AudioService _instance;

    Bus _masterBus;
    Bus _musicBus;
    Bus _ambientBus;
    Bus _sfxBus;
    Bus _uiBus;
    Bus _voiceBus;

    AudioEmitterPool _pool;
    Transform _clipRoot;
    AudioSource _oneShot2D;
    AudioSource _castChannel;
    AudioSource _voiceChannel;

    readonly System.Collections.Generic.List<WorldEmitter> _worldEmitters =
        new System.Collections.Generic.List<WorldEmitter>();

    [Obsolete("Use AudioPacketServices.Service or [Inject] AudioService")]
    public static AudioService Instance => _instance;

    public bool IsDisabled => _disabled;
    public bool IsCastPlaying => _castChannel != null && _castChannel.isPlaying;

    public float CastRemainingSec
    {
        get
        {
            if (_castChannel == null || !_castChannel.isPlaying || _castChannel.clip == null)
                return 0f;
            return Mathf.Max(0f, _castChannel.clip.length - _castChannel.time);
        }
    }

    sealed class WorldEmitter
    {
        public GameObject Go;
        public AudioSource Source;
        public AudioCategory Category;
        public float ReleaseAt;
        public int VoiceIndex = -1;
    }

    sealed class AudioServiceRunner : MonoBehaviour { }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        _pool = new AudioEmitterPool();
        EnsureClipHierarchy();
        AudioPacketServices.Bind(service: this);
        BindLegacyAudioManager();
        BindLegacyMusicManager();
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    void Start()
    {
        SetBuses();
        InterludeFmodCombatSoundResolver.WarmCombatBanks();
        LogStartupFmodBanks();
    }

    void Update()
    {
        ApplyBusVolumes();
        TickWorldEmitters();
        TickMusicLayers();
    }

    void BindLegacyAudioManager()
    {
        AudioManager legacy = GetComponent<AudioManager>();
        if (legacy != null)
            legacy.BindService(this);
    }

    void BindLegacyMusicManager()
    {
        MusicManager legacy = GetComponent<MusicManager>();
        if (legacy != null)
            legacy.BindService(this);
    }

    void EnsureClipHierarchy()
    {
        _clipRoot = transform.Find("ClipChannels");
        if (_clipRoot == null)
        {
            var host = new GameObject("ClipChannels");
            host.transform.SetParent(transform, false);
            _clipRoot = host.transform;
        }

        _oneShot2D = EnsureChildSource("OneShot2D", true);
        _castChannel = EnsureChildSource("Cast", true);
        _voiceChannel = EnsureChildSource("Voice", true);
    }

    AudioSource EnsureChildSource(string childName, bool spatial2D)
    {
        Transform existing = _clipRoot.Find(childName);
        GameObject go = existing != null ? existing.gameObject : new GameObject(childName);
        if (existing == null)
            go.transform.SetParent(_clipRoot, false);
        var src = go.GetComponent<AudioSource>();
        if (src == null)
            src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = spatial2D ? 0f : 1f;
        src.loop = false;
        return src;
    }

    void SetBuses()
    {
        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _ambientBus = RuntimeManager.GetBus("bus:/Ambient");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        _uiBus = RuntimeManager.GetBus("bus:/UI");
        _voiceBus = RuntimeManager.GetBus("bus:/SFX");
    }

    void ApplyBusVolumes()
    {
        bool muted = GameSettings.Mute || (_muteWhenNotFocused && !Application.isFocused);
        if (muted)
        {
            SetBusVolume(_masterBus, 0f);
            SetBusVolume(_musicBus, 0f);
            SetBusVolume(_ambientBus, 0f);
            SetBusVolume(_sfxBus, 0f);
            SetBusVolume(_uiBus, 0f);
            SetBusVolume(_voiceBus, 0f);
            return;
        }

        float master = _masterVolume * 0.75f * GameSettings.Master;
        SetBusVolume(_masterBus, master);
        SetBusVolume(_musicBus, _musicVolume * 0.7f * GameSettings.Music);
        SetBusVolume(_ambientBus, _ambientVolume * 0.45f * GameSettings.Ambient);
        SetBusVolume(_sfxBus, _sfxVolume * GameSettings.Sfx);
        SetBusVolume(_uiBus, _uiVolume * GameSettings.Sfx);
        SetBusVolume(_voiceBus, _voiceVolume * GameSettings.Voice);
    }

    static void SetBusVolume(Bus bus, float volume)
    {
        if (!bus.isValid())
            return;
        bus.setVolume(volume);
    }

    float CategoryVolume(AudioCategory category)
    {
        switch (category)
        {
            case AudioCategory.Music: return GameSettings.Music * _musicVolume;
            case AudioCategory.Ambient: return GameSettings.Ambient * _ambientVolume;
            case AudioCategory.Ui: return GameSettings.Sfx * _uiVolume;
            case AudioCategory.Voice: return GameSettings.Voice * _voiceVolume;
            default: return GameSettings.Sfx * _sfxVolume;
        }
    }

    void LogAudioError(string message)
    {
        if (!AudioErrorThrottle.TryEmit(message, Time.unscaledTime, once: false, out string suffix))
            return;
        FineageLog.Error(FineageLogCategory.World, message + suffix);
    }

    void LogAudioErrorOnce(string key, string message)
    {
        if (!AudioErrorThrottle.TryEmit(key, Time.unscaledTime, once: true, out _))
            return;
        FineageLog.Error(FineageLogCategory.World, message);
    }

    public void SyncInspectorVolumes(
        float master,
        float music,
        float ambient,
        float sfx,
        float ui,
        float voiceVolume,
        bool muteWhenNotFocused,
        bool disabled)
    {
        _masterVolume = master;
        _musicVolume = music;
        _ambientVolume = ambient;
        _sfxVolume = sfx;
        _uiVolume = ui;
        _voiceVolume = voiceVolume;
        _muteWhenNotFocused = muteWhenNotFocused;
        _disabled = disabled;
    }
}