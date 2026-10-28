using System;

/// <summary>
/// Trade session wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradePressOtherOkIncomingPacketServices
{
    public static TradeWindow Trade => IncomingPacketServices.Trade;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void ApplyPressOtherOk(TradePressOtherOkIncomingDto dto)
    {
    }
}
