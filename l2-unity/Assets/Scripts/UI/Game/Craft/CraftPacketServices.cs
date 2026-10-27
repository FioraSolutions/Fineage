/// <summary>
/// Craft UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 10).
/// </summary>
public static class CraftPacketServices
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
