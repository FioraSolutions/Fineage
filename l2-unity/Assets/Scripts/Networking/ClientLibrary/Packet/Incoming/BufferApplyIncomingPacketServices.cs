using System;

/// <summary>
/// Buff panel apply hub (#142 batch slice 161; UI hub slice 176).
/// </summary>
public static class BufferApplyIncomingPacketServices
{
    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    public static void Queue(Action action) =>
        UiApplyIncomingPacketServices.Queue(action);
}
