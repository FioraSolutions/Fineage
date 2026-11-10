using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{

    internal void ApplyCreatureSay(CreatureSayNetMsg say)
    {
        NetChatFeed.NoteSay(say.Name, say.Text, say.ChatType);
        // Chat log does not use ObjectId; overhead bubble does (#521).
        OverheadChatSay.Show(say.ObjectId, say.Text, say.ChatType);
    }


    /// <summary>Mobius SystemMessageId 1510 — skill resurrection ConfirmDlg.</summary>
    const int SysMsgResurrectionOffer = 1510;

    internal void ApplySystemMessage(SystemMessageNetMsg message)
    {
        string shown = NetSystemLines.Resolve(message.Id, message.Text, message.Params);
        NetChatFeed.NoteSystem(shown);

        if (message.Id == SysMsgResurrectionOffer)
            ShowResurrectionOffer(shown);
    }

    void ShowResurrectionOffer(string messageText)
    {
        SystemMessageWindow window = SystemMessagePacketServices.Window;
        if (window == null)
            return;

        window.ClearDialogHandlers();
        window.OnButtonOk += () =>
            ZoneCommands.Send(new RequestRestartPointCommand(ReviveRequestNetMsg.AcceptSkillResurrection));
        window.OnButtonClosed += () =>
            ZoneCommands.Send(new RequestRestartPointCommand(ReviveRequestNetMsg.DeclineSkillResurrection));
        window.ShowWindowDialogYesOrNot(string.IsNullOrEmpty(messageText)
            ? "Do you want to be resurrected?"
            : messageText);
    }

    /// <summary>A refused action now says why in chat instead of only logging.</summary>

    internal void ApplyPledgeStatus(PledgeStatusNetMsg status)
    {
        ClanPacketServices.ApplyStatus(new NetPledgeStatus
        {
            ClanId = status.ClanId,
            Name = status.Name ?? "",
            Level = status.Level,
            LeaderObjectId = status.LeaderObjectId,
            CrestId = status.CrestId,
            MemberCount = status.MemberCount,
            DissolvingExpiryUnixMs = status.DissolvingExpiryUnixMs,
            CharPenaltyExpiryUnixMs = status.CharPenaltyExpiryUnixMs
        });
    }


    internal void ApplyPledgeMemberList(PledgeMemberListNetMsg list)
    {
        PledgeMemberNet[] rows = list.Members ?? System.Array.Empty<PledgeMemberNet>();
        var members = new NetPledgeMember[rows.Length];
        for (int i = 0; i < rows.Length; i++)
        {
            PledgeMemberNet row = rows[i] ?? new PledgeMemberNet();
            members[i] = new NetPledgeMember
            {
                ObjectId = row.ObjectId,
                Name = row.Name ?? "",
                ClassId = row.ClassId,
                Level = row.Level,
                Online = row.Online,
                PledgeType = row.PledgeType
            };
        }

        ClanPacketServices.ApplyMembers(new NetPledgeMemberList
        {
            ClanId = list.ClanId,
            LeaderObjectId = list.LeaderObjectId,
            Members = members
        });
    }


    internal void ApplyClanAskJoin(ClanAskJoinNetMsg ask)
    {
        ClanPacketServices.ApplyAskJoin(new NetClanAskJoin
        {
            RequestorObjectId = ask.RequestorObjectId,
            RequestorName = ask.RequestorName ?? "",
            ClanId = ask.ClanId,
            ClanName = ask.ClanName ?? "",
            PledgeType = ask.PledgeType
        });
    }


    internal void ApplyClanJoinResult(ClanJoinResultNetMsg result)
    {
        ClanPacketServices.ApplyJoinResult(result.Response);
    }


    internal void ApplyPartyAskJoin(PartyAskJoinNetMsg ask)
    {
        var dto = AskJoinPartyDto.FromNet(ask.RequestorObjectId, ask.RequestorName ?? "", ask.ItemDistribution);
        if (_party != null) _party.ApplyAskJoinParty(dto);
        else AskJoinPartyApplyIncomingPacketServices.ApplyAskJoinParty(dto);
    }


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
