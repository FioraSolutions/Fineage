using System;

/// <summary>
/// Trade wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeDoneIncomingPacketServices
{
    public static void ApplyTradeDone() => GameIncomingPacketServices.Trade.HideWindow();
}
