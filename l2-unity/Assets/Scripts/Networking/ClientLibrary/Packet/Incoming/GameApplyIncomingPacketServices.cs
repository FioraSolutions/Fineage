using System;

/// <summary>
/// Game incoming packet handler access via UI domain facades and IncomingPacketServices queue fallback (#142 sub-slice 49).
/// </summary>
public static class GameApplyIncomingPacketServices
{
    public static ClanWindow Clan => ClanPacketServices.Clan;

    public static DealerWindow Dealer => MerchantPacketServices.Dealer;

    public static TradeWindow Trade => TradePacketServices.Trade;

    public static TradeRequestWindow TradeRequest => TradePacketServices.TradeRequest;

    public static QuestWindow Quest => QuestPacketServices.Quest;

    public static QuestListWindow QuestList => AuxiliaryGameUIPacketServices.QuestList;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);
}
