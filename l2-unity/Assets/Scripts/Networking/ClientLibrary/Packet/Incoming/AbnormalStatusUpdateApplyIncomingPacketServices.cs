/// <summary>
/// Apply facade (#142 batch slice 152; locator queue slice 258). Re-homes from <see cref="AbnormalStatusUpdateIncomingPacketServices"/>.
///
/// Abnormal status update wire packet handler entry (#142 batch slice 141).
/// Self/peer logic lives on <see cref="StatusApplyService"/>; this facade only reaches it.
/// </summary>

public static class AbnormalStatusUpdateApplyIncomingPacketServices
{
    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    public static void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateDto packet)
    {
        StatusApplyService status = ApplyServices.Status;
        if (status == null)
            throw new System.InvalidOperationException(
                "AbnormalStatusUpdate arrived before StatusApplyService was bound.");
        status.ApplyAbnormalStatusUpdate(packet);
    }

    public static void FlushPendingSelfAbnormals()
    {
        ApplyServices.Status?.FlushPendingSelfAbnormals();
    }

    public static void ApplyPeerAbnormalStatusUpdate(AbnormalStatusUpdateDto packet)
    {
        StatusApplyService status = ApplyServices.Status;
        if (status == null)
            throw new System.InvalidOperationException(
                "Peer AbnormalStatusUpdate arrived before StatusApplyService was bound.");
        status.ApplyPeerAbnormalStatusUpdate(packet);
    }

    public static void FlushPendingPeerAbnormals(int objectId)
    {
        ApplyServices.Status?.FlushPendingPeerAbnormals(objectId);
    }
}
