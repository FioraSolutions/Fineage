/// <summary>
/// Stop-move wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class StopMoveIncomingPacketServices
{
    public static void ApplyStop(StopMoveDto packet)
    {
        MoveCostLog.NoteStopPacket(packet != null ? packet.ObjId : 0);
        MovementPacketLatest.PutStop(packet);
    }
}
