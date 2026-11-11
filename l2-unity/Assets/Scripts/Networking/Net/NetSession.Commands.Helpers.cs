using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetSession
{
    public void SendNpcLink(int npcObjectId, string command)
    {
        string list;
        int location;
        switch (NetBypass.Classify(command, out list, out location))
        {
            case NetBypass.Teleport:
            case NetBypass.SupportMagic:
            case NetBypass.GiveBlessing:
                // Same bypass verb as V2; specialized opcodes come later.
                _ = SendBypassRequest(npcObjectId, NetBypass.Normalize(command));
                return;
            default:
                _ = SendBypassRequest(npcObjectId, NetBypass.Normalize(command));
                return;
        }
    }

    public Task SendRequestBuyItem(int npcObjectId, int listId, NetIdCount[] items) =>
        _commandSender.SendRequestBuyItemAsync(npcObjectId, listId, items);

    public Task SendRequestSellItem(int npcObjectId, NetIdCount[] items) =>
        _commandSender.SendRequestSellItemAsync(npcObjectId, items);

    public Task SendRequestMultiSellChoose(int npcObjectId, int listId, int entryId, int amount) =>
        _commandSender.SendRequestMultiSellChooseAsync(npcObjectId, listId, entryId, amount);

    public Task SendWarehouseDeposit(int npcObjectId, NetIdCount[] lines) =>
        _commandSender.SendWarehouseDepositAsync(npcObjectId, lines);

    public Task SendWarehouseWithdraw(int npcObjectId, NetIdCount[] lines) =>
        _commandSender.SendWarehouseWithdrawAsync(npcObjectId, lines);

    public Task SendRequestQuestList() =>
        _commandSender.SendRequestQuestListAsync();

    public Task SendRequestQuestAbort(int questId) =>
        _commandSender.SendRequestQuestAbortAsync(questId);

    public Task SendPartyInviteRequest(int targetObjectId, int itemDistribution = 0) =>
        _commandSender.SendPartyInviteRequestAsync(targetObjectId, itemDistribution);

    public Task SendPartyAnswerJoinRequest(int requestorObjectId, int response) =>
        _commandSender.SendPartyAnswerJoinRequestAsync(requestorObjectId, response);

    public Task SendPartyLeaveRequest() =>
        _commandSender.SendPartyLeaveRequestAsync();

    public Task SendPartyKickRequest(int targetObjectId) =>
        _commandSender.SendPartyKickRequestAsync(targetObjectId);

    public Task SendTradeRequest(int targetObjectId) =>
        _commandSender.SendTradeRequestAsync(targetObjectId);

    public Task SendTradeAnswerRequest(int requestorObjectId, int response) =>
        _commandSender.SendTradeAnswerRequestAsync(requestorObjectId, response);

    public Task SendTradeAddItemRequest(int inventoryObjectId, int count) =>
        _commandSender.SendTradeAddItemRequestAsync(inventoryObjectId, count);

    public Task SendTradeDoneRequest(int response) =>
        _commandSender.SendTradeDoneRequestAsync(response);

    public Task SendClanCreateRequest(string name) =>
        _commandSender.SendClanCreateRequestAsync(name);

    public Task SendClanInviteRequest(int targetObjectId) =>
        _commandSender.SendClanInviteRequestAsync(targetObjectId);

    public Task SendClanAnswerJoinRequest(int requestorObjectId, int response) =>
        _commandSender.SendClanAnswerJoinRequestAsync(requestorObjectId, response);

    public Task SendClanLeaveRequest() =>
        _commandSender.SendClanLeaveRequestAsync();

    public Task SendClanKickRequest(int targetObjectId) =>
        _commandSender.SendClanKickRequestAsync(targetObjectId);

    public Task SendClanDissolveRequest() =>
        _commandSender.SendClanDissolveRequestAsync();

    public Task SendClanCancelDissolveRequest() =>
        _commandSender.SendClanCancelDissolveRequestAsync();

    public Task SendTargetCancel() =>
        _commandSender.SendTargetCancelAsync();
}
