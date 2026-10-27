using System;

/// <summary>
/// Buffer panel wire packet handler access via UiIncomingPacketServices (#142 sub-slice 72).
/// </summary>
public static class BufferIncomingPacketServices
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

    public static void ApplyEtcStatusUpdate(EtcStatusUpdateDto packet) =>
        Queue(() =>
        {
            if (UiIncomingPacketServices.Buffer != null)
                UiIncomingPacketServices.Buffer.RefreshPenalty(packet);
        });
}
