
[IncomingGamePacket(GameServerPacketType.WhWithdrawList)]
public sealed class WhWithdrawListIncoming : IncomingWirePacket<WarehouseWithdrawListDto>
{
    public override void Apply(WarehouseWithdrawListDto packet) =>
        WarehouseIncomingPacketServices.ApplyWhWithdrawList(packet);
}
