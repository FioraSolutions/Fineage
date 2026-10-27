using System;
using UnityEngine;

/// <summary>
/// Thin transport abstraction for Zone V2 communication.
/// <see cref="ZoneV2Client"/> implements the wire protocol today over raw TCP.
/// Future implementations may use QUIC, WebSocket, or a gateway relay.
/// GameManager and other systems should depend on this interface,
/// not on <see cref="ZoneV2Client"/> directly.
/// </summary>
public interface IZoneTransport
{
    /// <summary>When true, login UI uses <see cref="BeginSession"/> instead of Interlude LoginClient.</summary>
    bool BypassesInterludeLogin { get; }

    /// <summary>When false, <see cref="GameClient.SendAuthLoginNow"/> is blocked (zone session owns auth).</summary>
    bool AllowsInterludeGameSocketAuth { get; }

    /// <summary>
    /// Handle an outgoing game command on the zone path.
    /// Returns true when the command was consumed (including intentional V2 no-ops).
    /// </summary>
    bool TrySendGameCommand(INetworkCommand command);

    bool Connected { get; }
    int SessionObjectId { get; }

    /// <summary>Begin a new session (connect + Hello + Auth + EnterWorld).</summary>
    void BeginSession(string account, string password);

    /// <summary>Send a movement intent to the zone server.</summary>
    void SendMove(Vector3 fromUnity, Vector3 toUnity);

    /// <summary>Graceful disconnect.</summary>
    void Disconnect();

    /// <summary>
    /// Fired on the main thread when the connection drops unexpectedly.
    /// Subscribers can show UI or attempt reconnect.
    /// </summary>
    event Action OnDisconnected;
}
