/// <summary>
/// Apply facade (#142 batch slice 152; locator queue slice 258). Re-homes from <see cref="AbnormalStatusUpdateIncomingPacketServices"/>.
///
/// Abnormal status update wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class AbnormalStatusUpdateApplyIncomingPacketServices
{
    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    public static void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet)
    {
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (Buffer == null)
                return;
            if (packet.FullEffectSnapshot)
                Buffer.RemoveAllEffects();
            foreach (var item in packet.ListEffect)
                Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
    }
}
