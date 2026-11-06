/// <summary>
/// Merchant UI access to game services with [Inject] wiring and live fallback (#142).
/// </summary>
public static class MerchantPacketServices
{
    private static DealerWindow _dealer;

    public static void Bind(DealerWindow dealer = null)
    {
        if (dealer != null)
            _dealer = dealer;
    }

    public static DealerWindow Dealer =>
        _dealer != null ? _dealer : DealerWindow.Instance;
}
