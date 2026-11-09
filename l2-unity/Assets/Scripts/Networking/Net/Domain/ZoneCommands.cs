using System;
using VContainer;

/// <summary>
/// The one way to send a game command.
///
/// <para>
/// This used to be <c>GameClient.Send</c>, reached through a <c>Game</c> property that dozens
/// of <c>*PacketServices</c> classes each re-declared. That property led to the Interlude
/// socket client — an <c>AsynchronousClient</c>, a Blowfish key, and a PlayOk handshake — none
/// of which Fineage Net uses. Sending a command only ever needed <see cref="IZoneTransport"/>:
/// this is that, with a container fallback for call sites that run before DI is up.
/// </para>
/// </summary>
public static class ZoneCommands
{
    static IZoneTransport _transport;

    /// <summary>Called by the DI scope once the transport exists.</summary>
    public static void Bind(IZoneTransport transport)
    {
        if (transport != null)
            _transport = transport;
    }

    public static void Clear()
    {
        _transport = null;
    }

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
    /// True while a zone session exists. Replaces <c>GameClient.IsConnected</c>, which asked
    /// whether the dead Interlude game socket was connected.
    /// </summary>
    public static bool IsConnected
    {
        get
        {
            IZoneTransport transport = Transport;
            return transport != null && transport.Connected;
        }
    }

    /// <summary>
    /// Send a command to the zone. Returns false when no transport is available, which means
    /// the command was not delivered — callers that care can surface that instead of assuming
    /// success.
    /// </summary>
    public static bool Send(INetworkCommand command)
    {
        if (command == null)
            return false;
        IZoneTransport transport = Transport;
        if (transport == null)
        {
            FineageLog.Warn(FineageLogCategory.GameNet, "TX dropped (no transport): " + command.GetType().Name);
            return false;
        }
        return transport.TrySendGameCommand(command);
    }
}
