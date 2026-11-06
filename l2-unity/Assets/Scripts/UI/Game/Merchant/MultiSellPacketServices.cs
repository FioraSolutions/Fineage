/// <summary>
/// Multi-sell UI access with [Inject] wiring and live fallback (#142).
/// </summary>
public static class MultiSellPacketServices
{
    private static MultiSellWindow _multiSell;

    public static void Bind(MultiSellWindow multiSell = null)
    {
        if (multiSell != null)
            _multiSell = multiSell;
    }

    public static MultiSellWindow MultiSell =>
        _multiSell != null ? _multiSell : MultiSellWindow.Instance;
}
