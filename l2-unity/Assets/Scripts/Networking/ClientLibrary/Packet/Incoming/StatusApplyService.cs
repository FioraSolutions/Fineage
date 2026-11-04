/// <summary>
/// Status effect/penalty applies, constructed by the container instead of reached
/// through static facades (#142 slice 3).
/// </summary>
public sealed class StatusApplyService
{
    public void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            BufferPanel buffer = LocatorApplyIncomingPacketServices.Buffer;
            if (buffer == null)
                return;
            if (packet.FullEffectSnapshot)
                buffer.RemoveAllEffects();
            foreach (var item in packet.ListEffect)
                buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });

    public void ApplyEtcStatusUpdate(EtcStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (LocatorApplyIncomingPacketServices.Buffer != null)
                LocatorApplyIncomingPacketServices.Buffer.RefreshPenalty(packet);
        });
}
