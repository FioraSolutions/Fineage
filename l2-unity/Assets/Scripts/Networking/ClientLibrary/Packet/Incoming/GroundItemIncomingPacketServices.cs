/// <summary>
/// Ground item and generic object-delete incoming handler access via WorldIncomingPacketServices (#142 sub-slice 66).
/// </summary>
public static class GroundItemIncomingPacketServices
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

    public static void ApplyDropItem(DropItemDto dto)
    {
        if (dto == null || GameWorld == null)
            return;

        GameWorld.SpawnOrUpdateGroundItem(
            dto.ObjectId,
            dto.ItemId,
            dto.Count,
            dto.Stackable,
            dto.UnityPos,
            dto.CharObjId);
    }

    public static void ApplyGetItem(GetItemDto dto)
    {
        if (dto == null || GameWorld == null)
            return;

        GameWorld.PickupGroundItem(dto.ObjectId, dto.PlayerId, dto.UnityPos);
    }

    public static void ApplyDeleteObject(DeleteObjectDto dto)
    {
        SpawnFrameBudget.Cancel(dto.ObjectId);
        if (GameWorld != null)
            GameWorld.DeleteObject(dto.ObjectId);
    }
}
