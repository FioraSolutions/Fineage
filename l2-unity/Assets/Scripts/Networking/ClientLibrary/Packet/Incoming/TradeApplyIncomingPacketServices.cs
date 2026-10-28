using System;

/// <summary>
/// Trade-session apply hub (#142 batch slice 159; game hub slice 167).
/// </summary>
public static class TradeApplyIncomingPacketServices
{
    public static TradeWindow Trade => GameApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest => GameApplyIncomingPacketServices.TradeRequest;

    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);
}
