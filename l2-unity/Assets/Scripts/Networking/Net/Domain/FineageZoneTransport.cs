using System;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Routes <see cref="IZoneTransport"/> calls to <see cref="NetSession"/>.
/// Keeps protocol mode checks out of GameManager, GameClient, and LoginWindow.
/// The Zone V2 wire path is no longer reachable from here.
/// </summary>
public sealed partial class FineageZoneTransport : IZoneTransport
{
    public event Action OnDisconnected;

    bool _disconnectNotified;

    /// <summary>
    /// Prefer DI-bound <see cref="NetSession"/> via <see cref="NetworkingPacketServices"/>;
    /// <see cref="NetSession.Ensure"/> only for early login before the binder runs.
    /// Static so party/trade user-command helpers can share the same locator.
    /// </summary>
    static NetSession Session => NetworkingPacketServices.Session ?? NetSession.Ensure();

    /// <summary>
    /// The local player profile. NetSession writes this when the character is selected and
    /// when the server sends UserInfo; the world scene reads it to spawn the player.
    /// </summary>
    public PlayerInfoInterlude PlayerInfo { get; set; } = new PlayerInfoInterlude();

    public bool Connected
    {
        get { return NetworkingPacketServices.Session != null; }
    }

    public int SessionObjectId
    {
        get
        {
            NetSession session = NetworkingPacketServices.Session;
            return session != null ? session.SessionObjectId : 0;
        }
    }

    public void BeginSession(string account, string password)
    {
        _disconnectNotified = false;
        Session.BeginLobby(account, password, registerIfMissing: true);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(toUnity);
        Session.SendMove(
            Mathf.RoundToInt(l2.x),
            Mathf.RoundToInt(l2.y),
            Mathf.RoundToInt(l2.z));
    }

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
        OnDisconnected?.Invoke();
    }

    /// <summary>Allow a later zone connect to raise disconnect again.</summary>
    internal void ResetDisconnectLatch() => _disconnectNotified = false;
}
