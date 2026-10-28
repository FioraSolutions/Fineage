using System;

/// <summary>
/// Buff panel apply hub (#142 batch slice 161; buffer orchestration slice 189).
/// </summary>
public static class BufferApplyIncomingPacketServices
{
    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    public static void Queue(Action action) =>
        UiApplyIncomingPacketServices.Queue(action);
}
