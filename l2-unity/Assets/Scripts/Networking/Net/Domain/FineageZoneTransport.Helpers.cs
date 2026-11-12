using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class FineageZoneTransport : IZoneTransport
{
    public void Disconnect()
    {
        NetSession session = NetworkingPacketServices.Session;
        if (session != null)
            UnityEngine.Object.Destroy(session.gameObject);
        NotifyDisconnected();
    }

    public void NotifyDisconnected()
    {
        if (_disconnectNotified)
            return;
        _disconnectNotified = true;
        // Stale local profile must not survive into a later reconnect / menu return.
        PlayerInfo = new PlayerInfoInterlude();
        OnDisconnected?.Invoke();
    }

    internal void ResetDisconnectLatch() => _disconnectNotified = false;
}
