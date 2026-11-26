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
    EtcStatusUpdateDto _etcStatus;
    bool _etcWaitingForBuffer;
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
            if (_pendingSelf != null)
            {
                AbnormalStatusUpdateDto pending = _pendingSelf;
                _pendingSelf = null;
                ApplySelf(pending);
            }

            FlushEtcStatus();
        });

    public void ApplyPeerAbnormalStatusUpdate(AbnormalStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() => ApplyPeer(packet));

    /// <summary>Mobius <c>EtcStatusUpdate</c> — weight, grade, death, and force-charge icons on the buffer tray.</summary>
    public void ApplyEtcStatusUpdate(EtcStatusUpdateDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (packet == null)
                return;
            _etcStatus = packet;
            _etcWaitingForBuffer = true;
            FlushEtcStatus();
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

        // Mobius AbnormalStatusUpdate *is* the whole abnormal row (fineage zone re-sends it on every apply and on
        // every toggle switch), so the toggle set is replaced, never merged — merging would leave a switched-off
        // toggle shining forever.
        SkillToggleState.Replace(packet.ListEffect);

        ApplySelfTargetEffects(packet);

        if (packet.ListEffect != null)
        {
            foreach (var item in packet.ListEffect)
                buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        }

        // AbnormalStatusUpdate replaces the abnormal row. RemoveAllEffects clears the tray, including
        // the force-charge icon, which is EtcStatusUpdate and not part of that row.
        if (packet.FullEffectSnapshot && _etcStatus != null)
            buffer.RefreshPenalty(_etcStatus);
    }

    void FlushEtcStatus()
    {
        if (!_etcWaitingForBuffer || _etcStatus == null)
            return;

        BufferPanel buffer = ResolveBuffer();
        if (buffer == null)
        {
            Debug.LogWarning(
                "[buffs] EtcStatusUpdate deferred — BufferPanel not ready charges=" +
                (_etcStatus.ForceCharge != null && _etcStatus.ForceCharge.Length > 1
                    ? _etcStatus.ForceCharge[1].ToString()
                    : "?"));
            return;
        }

        buffer.RefreshPenalty(_etcStatus);
        _etcWaitingForBuffer = false;
    }

    /// <summary>
    /// Mobius updateEffectIcons for self must drive <see cref="EntityAbnormalVisuals"/> (AVE_STEALTH, sleep FX, …),
    /// not only the buff tray. Peers already go through <see cref="ApplyPeer"/>.
    /// </summary>
    void ApplySelfTargetEffects(AbnormalStatusUpdateDto packet)
    {
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local == null)
            return;
        local.ReplaceTargetEffects(packet?.ListEffect);
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
