using System;
using VContainer;

public static partial class ZoneCommands
{
    public static void Clear()
    {
        _transport = null;
    }

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

    /// <summary>
    /// Local player profile owned by ACS <see cref="FineageZoneTransport"/> (#131).
    /// Kept off <see cref="IZoneTransport"/> so the contract can live in Fineage.Networking.
    /// </summary>
    public static PlayerInfoInterlude LocalPlayerInfo
    {
        get
        {
            FineageZoneTransport zone = Transport as FineageZoneTransport;
            return zone != null ? zone.PlayerInfo : default;
        }
        set
        {
            FineageZoneTransport zone = Transport as FineageZoneTransport;
            if (zone != null)
                zone.PlayerInfo = value;
        }
    }
}
