/// <summary>
/// Item enchant UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 23).
/// </summary>
public static class ItemEnchantPacketServices
{
    private static GameClient _game;
    private static EnchantWindow _enchant;

    public static void Bind(GameClient game = null, EnchantWindow enchant = null)
    {
        if (game != null)
            _game = game;
        if (enchant != null)
            _enchant = enchant;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static EnchantWindow Enchant =>
        _enchant != null ? _enchant : IncomingPacketActions.Enchant;
}
