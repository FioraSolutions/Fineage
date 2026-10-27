/// <summary>
/// Item enchant UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 23).
/// </summary>
public static class ItemEnchantPacketServices
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
