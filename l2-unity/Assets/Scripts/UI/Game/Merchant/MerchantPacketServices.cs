/// <summary>
/// Merchant UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 18).
/// </summary>
public static class MerchantPacketServices
{
    private static GameClient _game;
    private static DealerWindow _dealer;

    public static void Bind(GameClient game = null, DealerWindow dealer = null)
    {
        if (game != null)
            _game = game;
        if (dealer != null)
            _dealer = dealer;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static DealerWindow Dealer =>
        _dealer != null ? _dealer : IncomingPacketActions.Dealer;
}
