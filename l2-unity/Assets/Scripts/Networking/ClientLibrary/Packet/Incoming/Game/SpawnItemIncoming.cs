[IncomingGamePacket(GameServerPacketType.SpawnItem)]
public sealed class SpawnItemIncoming : IncomingWirePacket<SpawnItemDto>
{
    public override void Apply(SpawnItemDto dto)
    {
        if (dto == null || WorldIncomingPacketServices.GameWorld == null)
            return;

        WorldIncomingPacketServices.GameWorld.SpawnOrUpdateGroundItem(
            dto.ObjectId,
            dto.ItemId,
            dto.Count,
            dto.Stackable,
            dto.UnityPos,
            0);
    }
}
