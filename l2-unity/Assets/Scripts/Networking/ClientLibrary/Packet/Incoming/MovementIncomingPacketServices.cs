/// <summary>
/// Movement-related incoming handler access (#142 sub-slice 55).
/// Batches CharMoveToLocation / StopMove / MoveToPawn via <see cref="MovementPacketLatest"/>.
/// </summary>
public static class MovementIncomingPacketServices
{
    public static void ApplyMove(CharMoveToLocationDto packet)
    {
        MoveCostLog.NoteMovePacket(packet != null ? packet.ObjId : 0);
        MovementPacketLatest.PutMove(packet);
    }

    public static void ApplyStop(StopMoveDto packet)
    {
        MoveCostLog.NoteStopPacket(packet != null ? packet.ObjId : 0);
        MovementPacketLatest.PutStop(packet);
    }

    public static void ApplyPawn(MoveToPawnDto packet)
    {
        MoveCostLog.NotePawnPacket(packet != null ? packet.ObjId : 0);
        MovementPacketLatest.PutPawn(packet);
    }
}
