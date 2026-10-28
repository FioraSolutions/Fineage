using System;

/// <summary>
/// Short buff status wire packet handler entry (#142 batch slice 140).
/// </summary>
public static class ShortBuffStatusUpdateIncomingPacketServices
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
