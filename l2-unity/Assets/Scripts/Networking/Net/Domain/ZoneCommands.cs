using System;
using VContainer;

public static partial class ZoneCommands
{
    static IZoneTransport _transport;

    /// <summary>Called by the DI scope once the transport exists.</summary>


    public static IZoneTransport Transport
    {
        get
        {
            if (_transport != null)
                return _transport;
            if (!App.HasContainer)
                return null;
            try
            {
                return App.Resolve<IZoneTransport>();
            }
            catch
            {
                return null;
            }
        }
    }

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
