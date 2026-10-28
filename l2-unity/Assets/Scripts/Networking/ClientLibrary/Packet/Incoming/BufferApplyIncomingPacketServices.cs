using System;

/// <summary>
/// Buff panel apply hub (#142 batch slice 161; locator slice 170).
/// </summary>
public static class BufferApplyIncomingPacketServices
{
    public static BufferPanel Buffer => IncomingPacketLocatorApplyIncomingPacketServices.Buffer;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
