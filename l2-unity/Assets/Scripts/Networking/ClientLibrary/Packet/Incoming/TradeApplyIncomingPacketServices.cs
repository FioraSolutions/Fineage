using System;

/// <summary>
/// Trade-session apply hub (#142 batch slice 159).
/// </summary>
public static class TradeApplyIncomingPacketServices
{
    public static TradeWindow Trade => IncomingPacketLocatorApplyIncomingPacketServices.Trade;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
