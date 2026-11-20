using VContainer;

/// <summary>
/// Audio-layer access with [Inject] wiring and singleton fallback (#131 C2 / #142).
/// </summary>
public static class AudioPacketServices
{
    private static AudioManager _audio;
    private static AudioService _service;
    private static World _gameWorld;
    private static PlayerController _player;
    private static ThirdPersonListener _listener;

    static AudioPacketServices()
    {
        UiAudio.PlayUISound = soundName => Audio.PlayUISound(soundName);
    }

    public static void Bind(
        AudioManager audio = null,
        AudioService service = null,
        World gameWorld = null,
        PlayerController player = null,
        ThirdPersonListener listener = null)
    {
        if (audio != null)
            _audio = audio;
        if (service != null)
            _service = service;
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (player != null)
            _player = player;
        if (listener != null)
            _listener = listener;

        UiAudio.PlayUISound = soundName => Audio.PlayUISound(soundName);
    }

    public static AudioManager Audio =>
        _audio != null
            ? _audio
            : ResolveFromGame<AudioManager>() ?? LegacyAudioInstance();

    public static AudioService Service =>
        _service != null
            ? _service
            : ResolveFromGame<AudioService>() ?? LegacyServiceInstance();

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static PlayerController Player =>
        _player != null ? _player : NetworkingPacketServices.Player;

    public static ThirdPersonListener Listener =>
        _listener != null
            ? _listener
            : ResolveFromGame<ThirdPersonListener>() ?? LegacyListenerInstance();

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static AudioManager LegacyAudioInstance() => AudioManager.Instance;
    static AudioService LegacyServiceInstance() => AudioService.Instance;
    static ThirdPersonListener LegacyListenerInstance() => ThirdPersonListener.Instance;
#pragma warning restore CS0618
}
