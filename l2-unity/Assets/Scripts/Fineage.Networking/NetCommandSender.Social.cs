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

    public Task SendTradeDoneRequestAsync(int response)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeDoneRequest,
            NetCodec.EncodeTradeDoneRequest(new TradeDoneRequestNetMsg { Response = response }),
            _nextSeq());
    }

    public Task SendClanCreateRequestAsync(string name)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanCreateRequest,
            NetCodec.EncodeClanCreateRequest(new ClanCreateRequestNetMsg { Name = name ?? "" }),
            _nextSeq());
    }

    public Task SendClanInviteRequestAsync(int targetObjectId)
    {
        if (targetObjectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanInviteRequest,
            NetCodec.EncodeClanInviteRequest(new ClanInviteRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendClanAnswerJoinRequestAsync(int requestorObjectId, int response)
    {
        if (requestorObjectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanAnswerJoinRequest,
            NetCodec.EncodeClanAnswer(new ClanAnswerNetMsg { RequestorObjectId = requestorObjectId, Response = response }),
            _nextSeq());
    }

    public Task SendClanLeaveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.ClanLeaveRequest, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendClanKickRequestAsync(int targetObjectId)
    {
        if (targetObjectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanKickRequest,
            NetCodec.EncodeClanKickRequest(new ClanKickRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendClanDissolveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.ClanDissolveRequest, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendClanCancelDissolveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.ClanCancelDissolveRequest, Array.Empty<byte>(), _nextSeq());
    }

}
