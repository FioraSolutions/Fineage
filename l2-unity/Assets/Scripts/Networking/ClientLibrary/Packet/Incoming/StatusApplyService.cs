using VContainer;

/// <summary>
/// Status effect/penalty applies, constructed by the container instead of reached
/// through static facades (#142 slice 3). <see cref="BufferPanel"/> arrives through
/// <see cref="ILateBound{T}"/> (#142 slice 4).
/// </summary>
public sealed class StatusApplyService
{
    private readonly ILateBound<BufferPanel> _buffer;

    [Inject]
    public StatusApplyService(ILateBound<BufferPanel> buffer)
    {
        _buffer = buffer;
    }

    public void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            BufferPanel buffer = _buffer != null ? _buffer.Value : null;
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
            BufferPanel buffer = _buffer != null ? _buffer.Value : null;
            if (buffer != null)
                buffer.RefreshPenalty(packet);
        });
}
