/// <summary>
/// Ground-item wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="GroundItemIncomingPacketServices"/> (sub-slice 66).
/// </summary>
public static class DeleteObjectGroundIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static void ApplyDeleteObject(DeleteObjectDto dto)
    {
        SpawnFrameBudget.Cancel(dto.ObjectId);
        if (GameWorld != null)
            GameWorld.DeleteObject(dto.ObjectId);
    }
}
