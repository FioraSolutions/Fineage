[IncomingGamePacket(GameServerPacketType.DropItem)]
public sealed class DropItemIncoming : IncomingWirePacket<DropItemDto>
{
    public override void Apply(DropItemDto dto)
    {
        if (dto == null || WorldIncomingPacketServices.GameWorld == null)
            return;

        WorldIncomingPacketServices.GameWorld.SpawnOrUpdateGroundItem(
            dto.ObjectId,
            dto.ItemId,
            dto.Count,
            dto.Stackable,
            dto.UnityPos,
            dto.CharObjId);
    }
}
