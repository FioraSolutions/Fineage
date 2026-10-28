using System;

/// <summary>
/// Multi-sell UI apply hub (#142 batch slice 162; UI hub slice 167).
/// </summary>
public static class MultiSellApplyIncomingPacketServices
{
    public static MultiSellWindow MultiSell => UiApplyIncomingPacketServices.MultiSell;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}
