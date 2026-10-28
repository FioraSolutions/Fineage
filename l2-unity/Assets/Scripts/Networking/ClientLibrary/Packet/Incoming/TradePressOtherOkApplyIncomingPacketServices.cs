using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150; locator queue slice 256). Re-homes from <see cref="TradePressOtherOkIncomingPacketServices"/>.
/// 
/// Trade session wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradePressOtherOkApplyIncomingPacketServices
{
    public static TradeWindow Trade => LocatorApplyIncomingPacketServices.Trade;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyPressOtherOk(TradePressOtherOkIncomingDto dto)
    {
    }
}
