/// <summary>
/// Ground-item wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="GroundItemIncomingPacketServices"/> (sub-slice 66).
/// </summary>
public static class SpawnItemIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static void ApplySpawnItem(SpawnItemDto dto)
    {
        if (dto == null || GameWorld == null)
            return;

        GameWorld.SpawnOrUpdateGroundItem(
            dto.ObjectId,
            dto.ItemId,
            dto.Count,
            dto.Stackable,
            dto.UnityPos,
            0);
    }
}
