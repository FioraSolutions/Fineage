/// <summary>
/// World and lobby nameplate access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class NameplatePacketServices
{
    private static NameplatesManager _nameplates;
    private static LobbyNameplatesManager _lobbyNameplates;

    public static void Bind(
        NameplatesManager nameplates = null,
        LobbyNameplatesManager lobbyNameplates = null)
    {
        if (nameplates != null)
            _nameplates = nameplates;
        if (lobbyNameplates != null)
            _lobbyNameplates = lobbyNameplates;
    }

    public static NameplatesManager Nameplates =>
        _nameplates != null
            ? _nameplates
            : ResolveFromGame<NameplatesManager>() ?? LegacyNameplatesInstance();

    public static LobbyNameplatesManager LobbyNameplates =>
        _lobbyNameplates != null
            ? _lobbyNameplates
            : ResolveFromGame<LobbyNameplatesManager>() ?? LegacyLobbyNameplatesInstance();

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
    static NameplatesManager LegacyNameplatesInstance() => NameplatesManager.Instance;

    static LobbyNameplatesManager LegacyLobbyNameplatesInstance() => LobbyNameplatesManager.Instance;
#pragma warning restore CS0618
}
