using System;

/// <summary>
/// Multi-sell UI apply hub (#142 batch slice 162; locator slice 170).
/// </summary>
public static class MultiSellApplyIncomingPacketServices
{
    public static MultiSellWindow MultiSell => LocatorApplyIncomingPacketServices.MultiSell;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
