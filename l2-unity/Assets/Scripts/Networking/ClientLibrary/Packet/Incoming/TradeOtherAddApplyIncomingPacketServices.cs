using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150; locator queue slice 256). Re-homes from <see cref="TradeOtherAddIncomingPacketServices"/>.
/// 
/// Trade session wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeOtherAddApplyIncomingPacketServices
{
    public static TradeWindow Trade => LocatorApplyIncomingPacketServices.Trade;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyOtherAdd(TradeOtherAddIncomingDto dto)
    {
    }

    public static void ApplyOtherAdd(ItemInstance item) =>
        Queue(() => Trade?.ApplyPartnerOfferItem(item));
}
