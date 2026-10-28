using System;

/// <summary>
/// Trade-session apply hub (#142 batch slice 159; trade orchestration slice 187).
/// </summary>
public static class TradeApplyIncomingPacketServices
{
    public static TradeWindow Trade => LocatorApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest =>
        LocatorApplyIncomingPacketServices.TradeRequest;

    public static void Queue(Action action) =>
        GameApplyIncomingPacketServices.Queue(action);
}
