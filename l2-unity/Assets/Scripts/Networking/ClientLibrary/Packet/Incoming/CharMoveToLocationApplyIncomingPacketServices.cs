/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="CharMoveToLocationIncomingPacketServices"/>.
/// 
/// Char move-to-location wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class CharMoveToLocationApplyIncomingPacketServices
{
    public static void ApplyMove(CharMoveToLocationDto packet)
    {
        MoveCostLog.NoteMovePacket(packet != null ? packet.ObjId : 0);
        MovementPacketLatest.PutMove(packet);
    }
}
