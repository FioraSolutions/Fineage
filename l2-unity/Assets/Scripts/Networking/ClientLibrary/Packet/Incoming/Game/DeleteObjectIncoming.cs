[IncomingGamePacket(GameServerPacketType.DeleteObject)]
public sealed class DeleteObjectIncoming : IncomingPacket<DeleteObjectDto>
{
    public override DeleteObjectDto Read(PacketReader reader)
    {
        DeleteObjectDto dto = new DeleteObjectDto();
        dto.ObjectId = reader.ReadI();
        reader.ReadI();
        return dto;
    }

    public override void Apply(DeleteObjectDto dto)
    {
        GroundItemIncomingPacketServices.ApplyDeleteObject(dto);
    }
}
