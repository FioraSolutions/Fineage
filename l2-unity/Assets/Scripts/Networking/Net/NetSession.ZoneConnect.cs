using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    async Task ConnectZoneForHandoffAsync(HandoffMsg handoff)
    {
        // Null _zone before Dispose so a posted OnClosed from the previous
        // socket cannot tear down the transport we are about to open.
        NetTransport previous = _zone;
        _zone = null;
        previous?.Dispose();
        BumpZoneEpoch("zone-connect-start");
        _zoneConnecting = true;
        if (ZoneCommands.Transport is FineageZoneTransport latch)
            latch.ResetDisconnectLatch();
        NetTransport zone = null;
        try
        {
            zone = new NetTransport(OnZonePacket, reason =>
            {
                NetTransport closed = zone;
                Post(() => HandleZoneClosed(closed, reason));
            });
            _zone = zone;
            string host = string.IsNullOrWhiteSpace(handoff.ZoneHost) ? FineageWireConfig.ZoneHost : handoff.ZoneHost;
            int port = handoff.ZonePort > 0 ? handoff.ZonePort : FineageWireConfig.ZonePort;
            await _zone.ConnectAsync(host, port);
            _zoneConnecting = false;
            await SendZoneHelloAsync(_zone);
            await WaitUntil(() => _helloZone, 5f);
            await _zone.SendAsync(
                NetOpcode.ZoneAuth,
                NetCodec.EncodeZoneAuth(new ZoneAuthMsg { Account = handoff.Account, Token = handoff.Token }),
                NextSeq());
        }
        catch
        {
            _zoneConnecting = false;
            if (ReferenceEquals(_zone, zone))
                _zone = null;
            zone?.Dispose();
            throw;
        }
    }

    void HandleZoneClosed(NetTransport closed, string reason)
    {
        if (!ReferenceEquals(_zone, closed))
        {
            Debug.Log("[Net] zone " + reason + " (stale, ignored)");
            return;
        }

        Debug.Log("[Net] zone " + reason + " epoch=" + ZoneEpoch);
        // Drop the dead transport so Connected/ConnectionState flip immediately
        // and no further TX rides a half-closed socket (GPT reconnect P0).
        _zone = null;
        closed?.Dispose();
        BumpZoneEpoch("zone-tcp-closed");
        ZoneCommands.Transport?.NotifyDisconnected();
        if (CharSelectEnterWorld.IsEntering)
            CharSelectEnterWorld.Failed("Zone connection lost.");
    }

    /// <summary>
    /// Drop a half-open zone socket after a failed enter-world attempt.
    /// Leaves the login lobby connected so Start can be retried.
    /// </summary>
    internal void AbortZoneEnter()
    {
        _zoneConnecting = false;
        if (_zone == null)
            return;
        NetTransport zone = _zone;
        _zone = null;
        zone.Dispose();
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
