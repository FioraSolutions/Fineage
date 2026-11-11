using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
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
