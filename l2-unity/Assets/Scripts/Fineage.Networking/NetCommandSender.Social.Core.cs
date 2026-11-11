using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
    public Task SendPartyInviteRequestAsync(int targetObjectId, int itemDistribution)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PartyInviteRequest,
            NetCodec.EncodePartyInviteRequest(new PartyInviteRequestNetMsg
            {
                TargetObjectId = targetObjectId,
                ItemDistribution = itemDistribution
            }),
            _nextSeq());
    }

    public Task SendPartyAnswerJoinRequestAsync(int requestorObjectId, int response)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PartyAnswerJoinRequest,
            NetCodec.EncodePartyAnswerJoinRequest(new PartyAnswerJoinRequestNetMsg
            {
                RequestorObjectId = requestorObjectId,
                Response = response
            }),
            _nextSeq());
    }

    public Task SendPartyLeaveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.PartyLeaveRequest, NetCodec.EncodePartyLeaveRequest(), _nextSeq());
    }

    public Task SendPartyKickRequestAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PartyKickRequest,
            NetCodec.EncodePartyKickRequest(new PartyKickRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendTradeRequestAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeRequest,
            NetCodec.EncodeTradeRequest(new TradeRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendTradeAnswerRequestAsync(int requestorObjectId, int response)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeAnswerRequest,
            NetCodec.EncodeTradeAnswerRequest(new TradeAnswerRequestNetMsg
            {
                RequestorObjectId = requestorObjectId,
                Response = response
            }),
            _nextSeq());
    }

    public Task SendTradeAddItemRequestAsync(int inventoryObjectId, int count)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeAddItemRequest,
            NetCodec.EncodeTradeAddItemRequest(new TradeAddItemRequestNetMsg
            {
                InventoryObjectId = inventoryObjectId,
                Count = count
            }),
            _nextSeq());
    }
}
