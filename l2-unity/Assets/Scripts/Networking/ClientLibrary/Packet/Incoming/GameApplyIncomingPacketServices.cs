using System;

/// <summary>
/// Game incoming packet handler access via locator-backed domain facades (#142 sub-slice 49; slice 165).
/// </summary>
public static class GameApplyIncomingPacketServices
{
    public static ClanWindow Clan => IncomingPacketLocatorApplyIncomingPacketServices.Clan;

    public static DealerWindow Dealer => IncomingPacketLocatorApplyIncomingPacketServices.Dealer;

    public static TradeWindow Trade => IncomingPacketLocatorApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest =>
        IncomingPacketLocatorApplyIncomingPacketServices.TradeRequest;

    public static QuestWindow Quest => IncomingPacketLocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => IncomingPacketLocatorApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
