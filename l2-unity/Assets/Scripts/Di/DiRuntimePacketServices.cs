/// <summary>
/// DI runtime access to core clients with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 39).
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
        _game != null ? _game : IncomingPacketActions.Game;

    public static LoginClient Login =>
        _login != null ? _login : IncomingPacketActions.Login;

    public static GameManager Manager =>
        _manager != null ? _manager : IncomingPacketActions.Manager;
}
