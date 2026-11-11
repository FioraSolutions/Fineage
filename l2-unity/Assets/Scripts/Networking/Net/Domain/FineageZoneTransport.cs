using System;
using Fineage.Net.Contract;
using UnityEngine;

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





    /// <summary>Allow a later zone connect to raise disconnect again.</summary>
}
