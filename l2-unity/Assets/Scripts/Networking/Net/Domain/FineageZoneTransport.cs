using System;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class FineageZoneTransport : IZoneTransport
{
    public event Action OnDisconnected;

    bool _disconnectNotified;

    /// <summary>
    /// Bound <see cref="NetSession"/> from composition root via <see cref="NetworkingPacketServices"/>.
    /// Does not self-create — missing session is a DI/lifecycle error.
    /// </summary>
    static NetSession Session => NetworkingPacketServices.Session;

    /// <summary>
    /// The local player profile. NetSession writes this when the character is selected and
    /// when the server sends UserInfo; the world scene reads it to spawn the player.
    /// </summary>
    public PlayerInfoInterlude PlayerInfo { get; set; } = new PlayerInfoInterlude();

    /// <inheritdoc />
    /// Zone TCP is open — not merely that a NetSession MonoBehaviour exists.
    public bool Connected
    {
        get
        {
            NetSession session = NetworkingPacketServices.Session;
            return session != null && session.IsZoneSocketConnected;
        }
    }

    /// <inheritdoc />
    public bool HasSession => NetworkingPacketServices.Session != null;

    /// <inheritdoc />
    public ZoneConnectionState ConnectionState
    {
        get
        {
            NetSession session = NetworkingPacketServices.Session;
            return session != null
                ? session.ZoneConnectionState
                : ZoneConnectionState.Disconnected;
        }
    }

    public int SessionObjectId
    {
        get
        {
            NetSession session = NetworkingPacketServices.Session;
            return session != null ? session.SessionObjectId : 0;
        }
    }





    /// <summary>Allow a later zone connect to raise disconnect again.</summary>
}
