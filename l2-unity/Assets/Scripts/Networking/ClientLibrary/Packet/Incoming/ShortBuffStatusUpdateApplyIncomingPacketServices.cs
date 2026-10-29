using System;

/// <summary>
/// Short buff status wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 155; locator queue slice 259). Re-homes from <see cref="ShortBuffStatusUpdateIncomingPacketServices"/>.
/// </summary>
public static class ShortBuffStatusUpdateApplyIncomingPacketServices
{
    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyShortBuffStatusUpdate(ShortBuffStatusUpdateDto packet) =>
        Queue(() =>
        {
            if (Buffer == null || packet.Effect == null)
                return;
            var item = packet.Effect;
            Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
}
