
[IncomingGamePacket(GameServerPacketType.WhDepositList)]
public sealed class WhDepositListIncoming : IncomingWirePacket<WarehouseDepositListDto>
{
    public override void Apply(WarehouseDepositListDto packet) =>
        WhDepositListIncomingPacketServices.ApplyWhDepositList(packet);
}
