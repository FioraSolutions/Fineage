using VContainer;

/// <summary>
/// Background music access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class MusicPacketServices
{
    private static MusicManager _music;

    public static void Bind(MusicManager music = null)
    {
        if (music != null)
            _music = music;
    }

    public static MusicManager Music =>
        _music != null
            ? _music
            : ResolveFromGame<MusicManager>() ?? LegacyMusicInstance();

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
    static MusicManager LegacyMusicInstance() => MusicManager.Instance;
#pragma warning restore CS0618
}
