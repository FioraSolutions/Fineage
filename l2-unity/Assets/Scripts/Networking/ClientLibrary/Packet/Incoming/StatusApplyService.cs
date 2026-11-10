using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Status effect/penalty applies, constructed by the container instead of reached
/// through static facades (#142 slice 3). <see cref="BufferPanel"/> arrives through
/// <see cref="ILateBound{T}"/> (#142 slice 4).
/// Owns EnterWorld pending snapshots so restored buffs are not dropped before the tray binds.
/// </summary>
public sealed class StatusApplyService
{
    readonly ILateBound<BufferPanel> _buffer;
    AbnormalStatusUpdateDto _pendingSelf;
    readonly Dictionary<int, AbnormalStatusUpdateDto> _pendingPeers = new();

    [Inject]
    public StatusApplyService(ILateBound<BufferPanel> buffer)
    {
        _buffer = buffer;
    }

    public void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() => ApplySelf(packet));

    /// <summary>Called when <see cref="BufferPanel"/> binds so a queued enter snapshot can paint.</summary>
    public void FlushPendingSelfAbnormals() =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (_pendingSelf == null)
                return;
            AbnormalStatusUpdateDto pending = _pendingSelf;
            _pendingSelf = null;
            ApplySelf(pending);
        });

    public void ApplyPeerAbnormalStatusUpdate(AbnormalStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() => ApplyPeer(packet));

    /// <summary>Mobius <c>EtcStatusUpdate</c> — weight / chat-ban penalty icons on the buffer tray.</summary>
    public void ApplyEtcStatusUpdate(EtcStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (packet == null)
                return;
            BufferPanel buffer = ResolveBuffer();
            if (buffer != null)
                buffer.RefreshPenalty(packet);
        });

    /// <summary>Replay a PartySpelled snapshot that arrived before the entity was spawned.</summary>
    public void FlushPendingPeerAbnormals(int objectId) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (objectId <= 0 || !_pendingPeers.TryGetValue(objectId, out AbnormalStatusUpdateDto pending))
                return;
            _pendingPeers.Remove(objectId);
            ApplyPeer(pending);
        });

    void ApplySelf(AbnormalStatusUpdateDto packet)
    {
        if (packet == null)
            return;

        BufferPanel buffer = ResolveBuffer();
        if (buffer == null)
        {
            _pendingSelf = packet;
            Debug.LogWarning(
                $"[buffs] AbnormalStatusUpdate deferred — BufferPanel not ready " +
                $"(effects={packet.ListEffect?.Count ?? 0}, objectId={packet.ObjectId})");
            return;
        }

        if (packet.FullEffectSnapshot)
            buffer.RemoveAllEffects();
        if (packet.ListEffect == null)
            return;
        foreach (var item in packet.ListEffect)
            buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
    }

    void ApplyPeer(AbnormalStatusUpdateDto packet)
    {
        if (packet == null || packet.ObjectId <= 0)
            return;

        World world = WorldApplyIncomingPacketServices.GameWorld;
        Entity entity = world != null ? world.GetEntityNoLockSync(packet.ObjectId) : null;
        if (entity == null)
        {
            _pendingPeers[packet.ObjectId] = packet;
            return;
        }

        entity.ReplaceTargetEffects(packet.ListEffect);
        TargetWindow target = HudCorePacketServices.Target;
        if (target != null)
            target.RefreshEffectsIfSelected(packet.ObjectId);
    }

    BufferPanel ResolveBuffer()
    {
        if (_buffer != null && _buffer.Value != null)
            return _buffer.Value;
        return LocatorApplyIncomingPacketServices.Buffer;
    }
}
