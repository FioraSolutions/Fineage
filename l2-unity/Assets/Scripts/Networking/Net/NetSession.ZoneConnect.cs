using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    async Task ConnectZoneForHandoffAsync(HandoffMsg handoff)
    {
        _zone?.Dispose();
        _helloZone = false;
        if (ZoneCommands.Transport is FineageZoneTransport latch)
            latch.ResetDisconnectLatch();
        _zone = new NetTransport(OnZonePacket, reason => Post(() =>
        {
            Debug.Log("[Net] zone " + reason);
            ZoneCommands.Transport?.NotifyDisconnected();
        }));
        string host = string.IsNullOrWhiteSpace(handoff.ZoneHost) ? FineageWireConfig.ZoneHost : handoff.ZoneHost;
        int port = handoff.ZonePort > 0 ? handoff.ZonePort : FineageWireConfig.ZonePort;
        await _zone.ConnectAsync(host, port);
        await SendZoneHelloAsync(_zone);
        await WaitUntil(() => _helloZone, 5f);
        await _zone.SendAsync(
            NetOpcode.ZoneAuth,
            NetCodec.EncodeZoneAuth(new ZoneAuthMsg { Account = handoff.Account, Token = handoff.Token }),
            NextSeq());
    }

    async Task SendZoneHelloAsync(NetTransport transport)
    {
        await transport.SendAsync(
            NetOpcode.Hello,
            NetCodec.EncodeHello(new HelloMsg
            {
                ClientName = "unity-net-zone",
                FeatureFlags = ProtocolCapabilities.FeatureSession
                    | ProtocolCapabilities.FeatureLobby
                    | ProtocolCapabilities.FeatureWorldPresence
            }),
            NextSeq());
    }

    internal async System.Threading.Tasks.Task EnterWorldAsync()
    {
        if (_zone == null)
            return;
        await _zone.SendAsync(NetOpcode.EnterWorld, NetCodec.EncodeEnterWorld(new EnterWorldMsg()), NextSeq());
    }
}
