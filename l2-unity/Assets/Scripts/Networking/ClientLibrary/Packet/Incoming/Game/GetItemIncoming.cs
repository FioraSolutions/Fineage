[IncomingGamePacket(GameServerPacketType.GetItem)]
public sealed class GetItemIncoming : IncomingWirePacket<GetItemDto>
{
    public override void Apply(GetItemDto dto)
    {
        if (dto == null || WorldIncomingPacketServices.GameWorld == null)
            return;
        WorldIncomingPacketServices.GameWorld.PickupGroundItem(dto.ObjectId, dto.PlayerId, dto.UnityPos);
    }
}
