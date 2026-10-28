using System;

/// <summary>
/// Trade-session apply hub (#142 batch slice 159; locator slice 170).
/// </summary>
public static class TradeApplyIncomingPacketServices
{
    public static TradeWindow Trade => LocatorApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest =>
        LocatorApplyIncomingPacketServices.TradeRequest;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
