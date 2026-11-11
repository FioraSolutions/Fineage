/// <summary>
/// DI runtime access to core clients with [Inject] wiring and Networking/live fallback (#142).
/// </summary>
public static class DiRuntimePacketServices
{
    private static GameClient _game;
    private static LoginClient _login;
    private static GameManager _manager;

    public static void Bind(
        GameClient game = null,
        LoginClient login = null,
        GameManager manager = null)
    {
        if (game != null)
            _game = game;
        if (login != null)
            _login = login;
        if (manager != null)
            _manager = manager;
    }

    public static GameClient Game =>
        _game != null ? _game : ResolveGameClient();

    static GameClient ResolveGameClient()
    {
        if (App.HasContainer)
        {
            try
            {
                return App.Resolve<GameClient>();
            }
            catch
            {
            }
        }

#pragma warning disable CS0618 // Locator fallback until GameClient.Instance is removed
        return GameClient.Instance;
#pragma warning restore CS0618
    }

    public static LoginClient Login =>
        _login != null ? _login : NetworkingPacketServices.Login;

    public static GameManager Manager =>
        _manager != null ? _manager : NetworkingPacketServices.Manager;

    public static IGameManager ManagerContract =>
        Manager;
}
