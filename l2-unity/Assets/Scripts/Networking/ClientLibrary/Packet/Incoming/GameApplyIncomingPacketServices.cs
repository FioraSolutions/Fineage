using System;

/// <summary>
/// Game incoming packet handler access via domain apply hubs (#142 sub-slice 49; orchestration slice 170).
/// </summary>
public static class GameApplyIncomingPacketServices
{
    public static ClanWindow Clan => PledgeApplyIncomingPacketServices.Clan;

    public static DealerWindow Dealer => MerchantApplyIncomingPacketServices.Dealer;

    public static TradeWindow Trade => TradeApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest => TradeApplyIncomingPacketServices.TradeRequest;

    public static QuestWindow Quest => QuestApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => QuestApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
