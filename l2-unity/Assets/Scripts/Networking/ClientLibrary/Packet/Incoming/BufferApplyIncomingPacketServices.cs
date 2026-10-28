using System;

/// <summary>
/// Buff/status buffer panel apply hub (#142 batch slice 161).
/// </summary>
public static class BufferApplyIncomingPacketServices
{
    public static BufferPanel Buffer => IncomingPacketLocatorApplyIncomingPacketServices.Buffer;

    public static void Queue(Action action) => IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
