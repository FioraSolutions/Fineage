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
        World gameWorld = GameWorld;
        if (gameWorld != null)
            gameWorld.DeleteObject(dto.ObjectId);
    }
}
