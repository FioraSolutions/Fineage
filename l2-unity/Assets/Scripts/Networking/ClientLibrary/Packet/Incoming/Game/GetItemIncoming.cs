[IncomingGamePacket(GameServerPacketType.GetItem)]
public sealed class GetItemIncoming : IncomingWirePacket<GetItemDto>
{
    public override void Apply(GetItemDto dto)
    {
        if (dto == null || IncomingPacketServices.GameWorld == null)
            return;
        IncomingPacketServices.GameWorld.PickupGroundItem(dto.ObjectId, dto.PlayerId, dto.UnityPos);
    }
}
