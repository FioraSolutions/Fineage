using System;
using UnityEngine;

/// <summary>
/// Thin transport abstraction for Zone communication (#131).
/// Local player profile lives on the ACS concrete <c>FineageZoneTransport</c>
/// (see <c>ZoneCommands.LocalPlayerInfo</c>) so this contract stays cycle-safe.
/// </summary>
public interface IZoneTransport
{
    /// <summary>
    /// Handle an outgoing game command on the zone path.
    /// Returns true when the command was consumed (including intentional no-ops).
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
    /// Raise <see cref="OnDisconnected"/> once (remote close or intentional disconnect).
    /// Idempotent until the next successful zone connect.
    /// </summary>
    void NotifyDisconnected();

    /// <summary>
    /// Fired on the main thread when the connection drops unexpectedly or via <see cref="Disconnect"/>.
    /// </summary>
    event Action OnDisconnected;
}
