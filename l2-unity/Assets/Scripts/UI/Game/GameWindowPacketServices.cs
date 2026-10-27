/// <summary>
/// Root-level game UI window access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 24).
/// </summary>
public static class GameWindowPacketServices
{
    private static GameClient _game;

    public static void Bind(GameClient game = null)
    {
        if (game != null)
            _game = game;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;
}
