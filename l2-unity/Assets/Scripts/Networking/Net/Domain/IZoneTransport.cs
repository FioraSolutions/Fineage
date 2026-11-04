using System;
using UnityEngine;

/// <summary>
/// Thin transport abstraction for Zone communication.
/// <see cref="FineageZoneTransport"/> implements it over the Fineage Net protocol (raw TCP).
/// Future implementations may use QUIC, WebSocket, or a gateway relay.
/// GameManager and other systems should depend on this interface,
/// not on <see cref="FineageZoneTransport"/> directly.
/// </summary>
public interface IZoneTransport
{
    /// <summary>
    /// Handle an outgoing game command on the zone path.
    /// Returns true when the command was consumed (including intentional V2 no-ops).
    /// </summary>
    bool TrySendGameCommand(INetworkCommand command);

    bool Connected { get; }
    int SessionObjectId { get; }

    /// <summary>
    /// The local player's profile as the server last described it. Written by the session on
    /// connect and by <c>CharSelected</c>/<c>UserInfo</c>; read on world-scene load to spawn
    /// the player and by the clan book to stamp the local clan id.
    ///
    /// <para>
    /// This lived on <c>GameClient</c>, which meant a value the Net session writes every
    /// session was owned by the dead Interlude socket client.
    /// </para>
    /// </summary>
    PlayerInfoInterlude PlayerInfo { get; }

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
