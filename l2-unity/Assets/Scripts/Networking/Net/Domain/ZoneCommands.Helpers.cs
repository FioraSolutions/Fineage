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
}
