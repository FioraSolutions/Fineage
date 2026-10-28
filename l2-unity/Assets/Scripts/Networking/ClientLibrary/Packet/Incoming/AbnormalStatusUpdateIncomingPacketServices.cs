/// <summary>
/// Abnormal status update wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class AbnormalStatusUpdateIncomingPacketServices
{
    public static BufferPanel Buffer => WorldIncomingPacketServices.Buffer;

    public static void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            if (Buffer == null)
                return;
            foreach (var item in packet.ListEffect)
                Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
    }
}
