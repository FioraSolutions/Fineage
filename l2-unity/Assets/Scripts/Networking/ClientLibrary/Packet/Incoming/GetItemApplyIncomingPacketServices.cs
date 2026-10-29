/// <summary>
/// Ground-item wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="GroundItemIncomingPacketServices"/> (sub-slice 66).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="GetItemIncomingPacketServices"/>.
/// </summary>
public static class GetItemApplyIncomingPacketServices
{
    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static void ApplyGetItem(GetItemDto dto)
    {
        if (dto == null || GameWorld == null)
            return;

        GameWorld.PickupGroundItem(dto.ObjectId, dto.PlayerId, dto.UnityPos);
    }
}
