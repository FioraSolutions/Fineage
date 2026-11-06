/// <summary>
/// Ground-object removal apply, constructed by the container instead of reached through the
/// static <c>*ApplyIncomingPacketServices</c> relay (#142 slice 3 pilot).
/// </summary>
public sealed class DeleteObjectGroundApplyService
{
    public World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public void Apply(DeleteObjectDto dto)
    {
        SpawnFrameBudget.Cancel(dto.ObjectId);
        StorageNpc.getInstance().RemoveNpc(dto.ObjectId);
        StorageNpc.getInstance().RemoveChar(dto.ObjectId);
        World gameWorld = GameWorld;
        if (gameWorld == null)
            return;

        // Net has no GetItem packet: EntityDespawn after our PickupRequest must still play the pickup clip.
        if (NetSession.Instance != null &&
            NetSession.Instance.TryTakePendingPickup(dto.ObjectId, out int pickerId) &&
            gameWorld.GetEntityNoLockSync(dto.ObjectId) is ItemEntity item)
        {
            gameWorld.PickupGroundItem(dto.ObjectId, pickerId, item.transform.position);
            return;
        }

        gameWorld.DeleteObject(dto.ObjectId);
    }
}
