[IncomingGamePacket(GameServerPacketType.DropItem)]
public sealed class DropItemIncoming : IncomingWirePacket<DropItemDto>
{
    public override void Apply(DropItemDto dto)
    {
        if (dto == null || IncomingPacketServices.GameWorld == null)
            return;

        IncomingPacketServices.GameWorld.SpawnOrUpdateGroundItem(
            dto.ObjectId,
            dto.ItemId,
            dto.Count,
            dto.Stackable,
            dto.UnityPos,
            dto.CharObjId);
    }
}
