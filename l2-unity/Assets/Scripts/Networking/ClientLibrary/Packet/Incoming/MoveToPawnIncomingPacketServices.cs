/// <summary>
/// Move-to-pawn wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class MoveToPawnIncomingPacketServices
{
    public static void ApplyPawn(MoveToPawnDto packet)
    {
        MoveCostLog.NotePawnPacket(packet != null ? packet.ObjId : 0);
        MovementPacketLatest.PutPawn(packet);
    }
}
