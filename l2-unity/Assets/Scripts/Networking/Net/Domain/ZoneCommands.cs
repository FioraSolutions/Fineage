using System;
using VContainer;

public static partial class ZoneCommands
{
    static IZoneTransport _transport;

    /// <summary>Called by the DI scope once the transport exists.</summary>


    /// <summary>
    /// Bound by <see cref="Bind"/> from AppLifetimeScope. No silent App.Resolve fallback —
    /// missing bind means composition has not run (or was unbound).
    /// </summary>
    public static IZoneTransport Transport => _transport;

    /// <summary>
    /// True while the zone TCP socket is open. Replaces <c>GameClient.IsConnected</c>.
    /// Not the same as "a NetSession MonoBehaviour exists" — use <see cref="IZoneTransport.HasSession"/>.
    /// </summary>
    public static bool IsConnected
    {
        get
        {
            IZoneTransport transport = Transport;
            return transport != null && transport.Connected;
        }
    }

    /// <summary>Coarse zone lifecycle; Disconnected when no transport is bound.</summary>
    public static ZoneConnectionState ConnectionState
    {
        get
        {
            IZoneTransport transport = Transport;
            return transport != null
                ? transport.ConnectionState
                : ZoneConnectionState.Disconnected;
        }
    }

    /// <summary>
    /// Send a command to the zone. Returns false when no transport is available, which means
    /// the command was not delivered — callers that care can surface that instead of assuming
    /// success.
    /// </summary>
}
