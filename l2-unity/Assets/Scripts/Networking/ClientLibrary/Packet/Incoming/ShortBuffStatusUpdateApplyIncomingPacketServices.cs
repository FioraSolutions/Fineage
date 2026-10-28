using System;

/// <summary>
/// Short buff status wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="ShortBuffStatusUpdateIncomingPacketServices"/>.
/// </summary>
public static class ShortBuffStatusUpdateApplyIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyShortBuffStatusUpdate(ShortBuffStatusUpdateDto packet) =>
        Queue(() =>
        {
            if (UiIncomingPacketServices.Buffer == null || packet.Effect == null)
                return;
            var item = packet.Effect;
            UiIncomingPacketServices.Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
}
