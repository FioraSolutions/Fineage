using System;

/// <summary>
/// Buff/status buffer panel apply hub (#142 batch slice 161).
/// </summary>
public static class BufferApplyIncomingPacketServices
{
    public static BufferPanel Buffer => UiApplyIncomingPacketServices.Buffer;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}
