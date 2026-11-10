using UnityEngine;

/// <summary>
/// Apply facade (#142 batch slice 152; locator queue slice 258). Re-homes from <see cref="AbnormalStatusUpdateIncomingPacketServices"/>.
///
/// Abnormal status update wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class AbnormalStatusUpdateApplyIncomingPacketServices
{
    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    /// <summary>
    /// EnterWorld can deliver AbnormalStatusUpdate before BufferPanel exists. Hold the last self
    /// snapshot and flush when the tray binds — never silently drop restored buffs.
    /// </summary>
    static AbnormalStatusUpdateDto _pendingSelf;

    public static void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet)
    {
        QueueApplyIncomingPacketServices.Queue(() => ApplySelfNowOrPending(packet));
    }

    /// <summary>Called when <see cref="BufferPanel"/> binds so a queued enter snapshot can paint.</summary>
    public static void FlushPendingSelfAbnormals()
    {
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (_pendingSelf == null)
                return;
            AbnormalStatusUpdateDto pending = _pendingSelf;
            _pendingSelf = null;
            ApplySelfNowOrPending(pending);
        });
    }

    static void ApplySelfNowOrPending(AbnormalStatusUpdateDto packet)
    {
        if (packet == null)
            return;

        BufferPanel buffer = Buffer;
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

    /// <summary>
    /// Peer / NPC PartySpelled snapshot → entity <see cref="Entity.TargetEffects"/> + TargetWindow refresh.
    /// Must not touch <see cref="BufferPanel"/>.
    /// </summary>
    public static void ApplyPeerAbnormalStatusUpdate(AbnormalStatusUpdateDto packet)
    {
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            if (packet == null || packet.ObjectId <= 0)
                return;
            World world = WorldApplyIncomingPacketServices.GameWorld;
            Entity entity = world != null ? world.GetEntityNoLockSync(packet.ObjectId) : null;
            if (entity == null)
            {
                Debug.LogWarning(
                    $"[buffs] peer AbnormalStatusUpdate dropped — entity {packet.ObjectId} not spawned yet " +
                    $"(effects={packet.ListEffect?.Count ?? 0})");
                return;
            }
            entity.ReplaceTargetEffects(packet.ListEffect);
            TargetWindow target = HudCorePacketServices.Target;
            if (target != null)
                target.RefreshEffectsIfSelected(packet.ObjectId);
        });
    }
}
