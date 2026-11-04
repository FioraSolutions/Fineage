/// <summary>
/// Trade UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 19).
/// </summary>
public static class TradePacketServices
{
    private static TradeWindow _trade;
    private static TradeRequestWindow _tradeRequest;

    public static void Bind(
        TradeWindow trade = null,
        TradeRequestWindow tradeRequest = null)
    {
        if (trade != null)
            _trade = trade;
        if (tradeRequest != null)
            _tradeRequest = tradeRequest;
    }

    public static TradeWindow Trade =>
        _trade != null ? _trade : IncomingPacketActions.Trade;

    public static TradeRequestWindow TradeRequest =>
        _tradeRequest != null ? _tradeRequest : IncomingPacketActions.TradeRequest;
}
