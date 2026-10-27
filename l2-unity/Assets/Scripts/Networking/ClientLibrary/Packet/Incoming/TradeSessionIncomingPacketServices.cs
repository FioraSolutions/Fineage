using System;

/// <summary>
/// Trade session wire packet handler access (#142 sub-slice 56).
/// Complements trade open/close handlers wired in sub-slice 49 (PR #210).
/// </summary>
public static class TradeSessionIncomingPacketServices
{
    public static TradeWindow Trade => IncomingPacketServices.Trade;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void ApplyOwnAdd(TradeOwnAddIncomingDto dto)
    {
    }

    public static void ApplyOtherAdd(TradeOtherAddIncomingDto dto)
    {
    }

    public static void ApplyUpdate(TradeUpdateIncomingDto dto)
    {
    }

    public static void ApplyPressOwnOk(TradePressOwnOkIncomingDto dto)
    {
    }

    public static void ApplyPressOtherOk(TradePressOtherOkIncomingDto dto)
    {
    }
}
