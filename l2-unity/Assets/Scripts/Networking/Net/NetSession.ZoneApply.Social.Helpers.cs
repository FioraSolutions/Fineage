using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    internal void ApplyPartyJoinResult(PartyJoinResultNetMsg result)
    {
        if (_party != null) _party.ApplyPartyJoinResult(result.Response);
        else PartyJoinResultApplyIncomingPacketServices.ApplyPartyJoinResult(result.Response);
    }

    internal void ApplyPartyMemberList(PartyMemberListNetMsg list)
    {
        PartyMemberEntryNet[] members = list.Members ?? System.Array.Empty<PartyMemberEntryNet>();
        var rows = new NetPartyMemberEntry[members.Length];
        for (int i = 0; i < members.Length; i++)
        {
            PartyMemberEntryNet row = members[i] ?? new PartyMemberEntryNet();
            rows[i] = new NetPartyMemberEntry
            {
                ObjectId = row.ObjectId,
                Name = row.Name ?? "",
                ClassId = row.ClassId,
                Level = row.Level,
                CurHp = row.CurHp,
                MaxHp = row.MaxHp,
                CurMp = row.CurMp,
                MaxMp = row.MaxMp
            };
        }

        var roster = new NetPartyMemberList
        {
            LeaderObjectId = list.LeaderObjectId,
            PartyId = list.PartyId,
            Members = rows
        };
        if (_party != null) _party.ApplyPartyMemberList(roster);
        else PartyMemberListApplyIncomingPacketServices.ApplyPartyMemberList(roster);
    }

    internal void ApplyTradeAsk(TradeAskNetMsg ask)
    {
        SendTradeRequestDto dto = SendTradeRequestDto.FromNet(ask.RequestorObjectId, ask.RequestorName ?? "");
        if (_tradeRequest != null) _tradeRequest.ApplySendTradeRequest(dto);
        else SendTradeRequestApplyIncomingPacketServices.ApplySendTradeRequest(dto);
    }

    internal void ApplyTradeRequestResult(TradeRequestResultNetMsg result)
    {
        if (result.Response == 0)
            NetChatFeed.NoteSystem("Trade request declined.");
    }

    internal void ApplyTradeStart(TradeStartNetMsg start)
    {
        TradeStartDto dto = TradeStartDto.FromNet(start.PartnerObjectId, start.PartnerName ?? "");
        if (_trade != null) _trade.ApplyStart(dto);
        else TradeStartApplyIncomingPacketServices.ApplyTradeStart(dto);
    }

    internal void ApplyTradeOfferUpdate(TradeOfferUpdateNetMsg update)
    {
        int count = (int)update.Count;
        ItemInstance item = new ItemInstance(
            update.InventoryObjectId,
            update.ItemId,
            ItemLocation.Trade,
            0,
            count < 1 ? 1 : count,
            ItemCategory.Item,
            false,
            ItemSlot.none,
            update.EnchantLevel,
            9999);
        if (update.IsOwn != 0)
            if (_trade != null) _trade.ApplyOwnAdd(item);
            else TradeOwnAddApplyIncomingPacketServices.ApplyOwnAdd(item);
        else
            if (_trade != null) _trade.ApplyOtherAdd(item);
            else TradeOtherAddApplyIncomingPacketServices.ApplyOtherAdd(item);
    }

    internal void ApplyTradeConfirmState(TradeConfirmStateNetMsg state)
    {
        if (_trade != null) _trade.ApplyPressOwnOk(state.SelfConfirmed != 0);
        else TradePressOwnOkApplyIncomingPacketServices.ApplyPressOwnOk(state.SelfConfirmed != 0);
        if (_trade != null) _trade.ApplyPressOtherOk(state.PartnerConfirmed != 0);
        else TradePressOtherOkApplyIncomingPacketServices.ApplyPressOtherOk(state.PartnerConfirmed != 0);
    }

    internal void ApplyTradeFinished(TradeFinishedNetMsg _)
    {
        if (_trade != null) _trade.ApplyDone();
        else TradeDoneApplyIncomingPacketServices.ApplyTradeDone();
    }
}
