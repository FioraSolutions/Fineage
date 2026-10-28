/// <summary>
/// Move-to-pawn wire packet handler entry (#142 batch slice 141).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="MoveToPawnIncomingPacketServices"/>.
/// </summary>
public static class MoveToPawnApplyIncomingPacketServices
{
    public static void ApplyPawn(MoveToPawnDto packet)
    {
        MoveCostLog.NotePawnPacket(packet != null ? packet.ObjId : 0);
        MovementPacketLatest.PutPawn(packet);
    }
}
