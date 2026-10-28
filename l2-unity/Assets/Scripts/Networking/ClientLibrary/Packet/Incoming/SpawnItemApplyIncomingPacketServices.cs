/// <summary>
/// Ground-item wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="GroundItemIncomingPacketServices"/> (sub-slice 66).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="SpawnItemIncomingPacketServices"/>.
/// </summary>
public static class SpawnItemApplyIncomingPacketServices
{
    public static World GameWorld => GroundItemApplyIncomingPacketServices.GameWorld;

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
