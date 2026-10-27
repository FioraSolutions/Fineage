[IncomingGamePacket(GameServerPacketType.SpawnItem)]
public sealed class SpawnItemIncoming : IncomingWirePacket<SpawnItemDto>
{
    public override void Apply(SpawnItemDto dto) =>
        GroundItemIncomingPacketServices.ApplySpawnItem(dto);
}
