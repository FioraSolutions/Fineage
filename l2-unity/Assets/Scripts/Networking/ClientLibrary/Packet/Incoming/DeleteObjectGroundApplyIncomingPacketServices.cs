/// <summary>
/// Ground-item wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="GroundItemIncomingPacketServices"/> (sub-slice 66).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="DeleteObjectGroundIncomingPacketServices"/>.
/// </summary>
public static class DeleteObjectGroundApplyIncomingPacketServices
{
    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static void ApplyDeleteObject(DeleteObjectDto dto)
    {
        SpawnFrameBudget.Cancel(dto.ObjectId);
        if (GameWorld != null)
            GameWorld.DeleteObject(dto.ObjectId);
    }
}
