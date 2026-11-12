using System;
using VContainer;

public static partial class ZoneCommands
{
    public static void Bind(IZoneTransport transport)
    {
        if (transport != null)
            _transport = transport;
    }

    /// <summary>
    /// Clear the bound transport when the composition root is destroyed so a rebuilt
    /// scope cannot keep sending through a disposed instance (GPT A03).
    /// </summary>
    public static void Unbind(IZoneTransport transport = null)
    {
        if (transport != null && !ReferenceEquals(_transport, transport))
            return;
        _transport = null;
    }
}
