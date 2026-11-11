using System;
using VContainer;

public static partial class ZoneCommands
{
    public static void Bind(IZoneTransport transport)
    {
        if (transport != null)
            _transport = transport;
    }
}
