using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class FineageZoneTransport
{
    bool TrySendEconomySocialCommands(INetworkCommand command)
    {
        RequestBuyItemCommand netBuy = command as RequestBuyItemCommand;
        if (netBuy != null)
        {
            // Admin GM lists open with NpcObjectId=0; prefer that over a stale merchant id.
            int shopNpc = NetNpcSession.ShopNpcObjectId;
            Session.SendRequestBuyItem(
                shopNpc,
                netBuy.ListId > 0 ? netBuy.ListId : NetNpcSession.ShopListId,
                NetNpcOrders.ItemCounts(netBuy.BuyList));
            return true;
        }

        RequestSellItemCommand netSell = command as RequestSellItemCommand;
        if (netSell != null)
        {
            Session.SendRequestSellItem(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netSell.SellList));
            return true;
        }

        MultiSellChooseCommand netMulti = command as MultiSellChooseCommand;
        if (netMulti != null)
        {
            Session.SendRequestMultiSellChoose(
                NetNpcSession.ShopNpcObjectId,
                netMulti.ListId,
                netMulti.EntryId,
                netMulti.Amount);
            return true;
        }

        SendWarehouseDepositListCommand netDeposit = command as SendWarehouseDepositListCommand;
        if (netDeposit != null)
        {
            Session.SendWarehouseDeposit(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netDeposit.SellList));
            return true;
        }

        SendWarehouseWithdrawListCommand netWithdraw = command as SendWarehouseWithdrawListCommand;
        if (netWithdraw != null)
        {
            Session.SendWarehouseWithdraw(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netWithdraw.SellList));
            return true;
        }

        if (command is RequestQuestListCommand)
        {
            Session.SendRequestQuestList();
            return true;
        }

        RequestQuestAbortCommand netAbortQuest = command as RequestQuestAbortCommand;
        if (netAbortQuest != null)
        {
            Session.SendRequestQuestAbort(netAbortQuest.QuestId);
            return true;
        }

        RequestJoinPartyCommand netJoinParty = command as RequestJoinPartyCommand;
        if (netJoinParty != null)
        {
            Session.SendPartyInviteRequest(netJoinParty.TargetObjectId, netJoinParty.ItemDistribution);
            return true;
        }

        RequestAnswerJoinPartyCommand netAnswerParty = command as RequestAnswerJoinPartyCommand;
        if (netAnswerParty != null)
        {
            if (netAnswerParty.RequestorObjectId > 0)
                Session.SendPartyAnswerJoinRequest(netAnswerParty.RequestorObjectId, netAnswerParty.Response);
            return true;
        }

        if (command is RequestWithdrawalPartyCommand)
        {
            Session.SendPartyLeaveRequest();
            return true;
        }

        RequestOustPartyMemberCommand netOustParty = command as RequestOustPartyMemberCommand;
        if (netOustParty != null)
        {
            Session.SendPartyKickRequest(netOustParty.TargetObjectId);
            return true;
        }

        // Mobius clan window: the seven pledge opcodes travel as their own request opcodes.
        ClanCreateRequestCommand netClanCreate = command as ClanCreateRequestCommand;
        if (netClanCreate != null)
        {
            Session.SendClanCreateRequest(netClanCreate.Name);
            return true;
        }

        // RequestJoinPledge and ClanInviteRequestCommand are the same Mobius opcode: the clan
        // owner invites the target, and a clanless player asks the owner to take them in.
        ClanInviteRequestCommand netClanInvite = command as ClanInviteRequestCommand;
        if (netClanInvite != null)
        {
            Session.SendClanInviteRequest(netClanInvite.TargetObjectId);
            return true;
        }

        RequestJoinPledgeCommand netClanJoin = command as RequestJoinPledgeCommand;
        if (netClanJoin != null)
        {
            Session.SendClanInviteRequest(netClanJoin.ObjectId);
            return true;
        }

        ClanAnswerJoinRequestCommand netClanAnswer = command as ClanAnswerJoinRequestCommand;
        if (netClanAnswer != null)
        {
            if (netClanAnswer.RequestorObjectId > 0)
                Session.SendClanAnswerJoinRequest(netClanAnswer.RequestorObjectId, netClanAnswer.Response);
            return true;
        }

        RequestWithdrawPledgeCommand netClanLeave = command as RequestWithdrawPledgeCommand;
        if (netClanLeave != null)
        {
            Session.SendClanLeaveRequest();
            return true;
        }

        ClanLeaveRequestCommand netClanLeaveExplicit = command as ClanLeaveRequestCommand;
        if (netClanLeaveExplicit != null)
        {
            Session.SendClanLeaveRequest();
            return true;
        }

        ClanKickRequestCommand netClanKick = command as ClanKickRequestCommand;
        if (netClanKick != null)
        {
            Session.SendClanKickRequest(netClanKick.TargetObjectId);
            return true;
        }

        // RequestOustPledgeMember is the Interlude name and carries a name; the Net opcode
        // takes the object id, so resolve it through the roster the zone just sent.
        RequestOustPledgeMemberCommand netClanOust = command as RequestOustPledgeMemberCommand;
        if (netClanOust != null)
        {
            NetPledgeMember member;
            if (NetClanBook.Shared.TryMemberByName(netClanOust.MemberName, out member))
                Session.SendClanKickRequest(member.ObjectId);
            return true;
        }

        // Mobius RequestTargetCanceld: an empty body. The client clears its own selection in
        // TargetManager; this is what tells the zone, so the next untargeted skill stops aiming there.
        if (command is RequestTargetCanceldCommand)
        {
            Session.SendTargetCancel();
            return true;
        }

        ClanDissolveRequestCommand netClanDissolve = command as ClanDissolveRequestCommand;
        if (netClanDissolve != null)
        {
            Session.SendClanDissolveRequest();
            return true;
        }

        ClanCancelDissolveRequestCommand netClanCancel = command as ClanCancelDissolveRequestCommand;
        if (netClanCancel != null)
        {
            Session.SendClanCancelDissolveRequest();
            return true;
        }

        RequestTradeCommand netRequestTrade = command as RequestTradeCommand;        if (netRequestTrade != null)
        {
            Session.SendTradeRequest(netRequestTrade.TargetObjectId);
            return true;
        }

        AnswerTradeRequestCommand netAnswerTrade = command as AnswerTradeRequestCommand;
        if (netAnswerTrade != null)
        {
            if (netAnswerTrade.RequestorObjectId > 0)
                Session.SendTradeAnswerRequest(netAnswerTrade.RequestorObjectId, netAnswerTrade.Response);
            return true;
        }

        AddTradeItemCommand netAddTrade = command as AddTradeItemCommand;
        if (netAddTrade != null)
        {
            Session.SendTradeAddItemRequest(netAddTrade.ObjectId, netAddTrade.Count);
            return true;
        }

        TradeDoneCommand netTradeDone = command as TradeDoneCommand;
        if (netTradeDone != null)
        {
            Session.SendTradeDoneRequest(netTradeDone.Response);
            return true;
        }

        RequestUserCommandCommand netUserCommand = command as RequestUserCommandCommand;
        if (netUserCommand != null)
        {
            if (TrySendNetPartyUserCommand(netUserCommand.Id))
                return true;
            if (TrySendNetTradeUserCommand(netUserCommand.Id))
                return true;
        }

        // No Net opcode exists for this command yet. Returning true still drops it, but say
        // so: returning true silently is how a clan invite or a recipe-book open vanishes
        // with no trace, and the caller has no way to tell it apart from a delivered command.
        FineageLog.Warn(FineageLogCategory.GameNet, "TX dropped (no Net opcode): " + command.GetType().Name);
        return true;
        return false;
    }}

}
