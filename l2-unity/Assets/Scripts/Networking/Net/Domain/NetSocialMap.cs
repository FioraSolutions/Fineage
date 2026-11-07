using Fineage.Net.Contract;

/// <summary>Fineage Net party/trade wire → Interlude-shaped social DTOs.</summary>
public static class NetSocialMap
{
    public static AskJoinPartyDto ToAskJoinPartyDto(NetPartyAskJoin ask) =>
        AskJoinPartyDto.FromNet(ask.RequestorObjectId, ask.RequestorName, ask.ItemDistribution);

    /// <summary>One-line party roster summary for chat feedback. Display only — no wire codec.</summary>
    public static string FormatPartyMemberList(in NetPartyMemberList list)
    {
        if (list.Members == null || list.Members.Length == 0)
            return "Party: (none)";
        var names = new string[list.Members.Length];
        for (int i = 0; i < list.Members.Length; i++)
            names[i] = list.Members[i].Name ?? "?";
        return "Party (" + list.Members.Length + "): " + string.Join(", ", names);
    }

    public static SendTradeRequestDto ToSendTradeRequestDto(NetTradeAsk ask) =>
        SendTradeRequestDto.FromNet(ask.RequestorObjectId, ask.RequestorName);

    public static TradeStartDto ToTradeStartDto(NetTradeStart start) =>
        TradeStartDto.FromNet(start.PartnerObjectId, start.PartnerName);

    public static ItemInstance ToTradeOfferItem(in NetTradeOfferUpdate update)
    {
        return new ItemInstance(
            update.InventoryObjectId,
            update.ItemId,
            ItemLocation.Trade,
            0,
            update.Count < 1 ? 1 : update.Count,
            ItemCategory.Item,
            false,
            ItemSlot.none,
            update.EnchantLevel,
            9999);
    }
}
