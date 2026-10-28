using System;

/// <summary>
/// Multi-sell window apply hub (#142 batch slice 162).
/// </summary>
public static class MultiSellApplyIncomingPacketServices
{
    public static MultiSellWindow MultiSell => UiApplyIncomingPacketServices.MultiSell;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
