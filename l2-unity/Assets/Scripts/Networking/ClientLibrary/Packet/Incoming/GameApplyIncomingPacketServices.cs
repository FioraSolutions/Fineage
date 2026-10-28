using System;

/// <summary>
/// Game incoming packet handler access via domain apply hubs (#142 sub-slice 49; locator hub slice 250).
/// </summary>
public static class GameApplyIncomingPacketServices
{
    public static ClanWindow Clan => LocatorApplyIncomingPacketServices.Clan;

    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static TradeWindow Trade => LocatorApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest =>
        LocatorApplyIncomingPacketServices.TradeRequest;

    public static QuestWindow Quest => LocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => LocatorApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
