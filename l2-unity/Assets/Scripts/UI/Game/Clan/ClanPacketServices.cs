/// <summary>
/// Clan UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 16).
/// </summary>
public static class ClanPacketServices
{
    private static GameClient _game;
    private static ClanWindow _clan;

    public static void Bind(GameClient game = null, ClanWindow clan = null)
    {
        if (game != null)
            _game = game;
        if (clan != null)
            _clan = clan;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static ClanWindow Clan =>
        _clan != null ? _clan : IncomingPacketActions.Clan;
}
