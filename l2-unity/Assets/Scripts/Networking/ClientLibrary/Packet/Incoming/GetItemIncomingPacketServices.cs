/// <summary>
/// Ground-item wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="GroundItemIncomingPacketServices"/> (sub-slice 66).
/// </summary>
public static class GetItemIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static void ApplyGetItem(GetItemDto dto)
    {
        if (dto == null || GameWorld == null)
            return;

        GameWorld.PickupGroundItem(dto.ObjectId, dto.PlayerId, dto.UnityPos);
    }
}
