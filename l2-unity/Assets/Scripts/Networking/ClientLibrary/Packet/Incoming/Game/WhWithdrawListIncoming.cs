
[IncomingGamePacket(GameServerPacketType.WhWithdrawList)]
public sealed class WhWithdrawListIncoming : IncomingWirePacket<WarehouseWithdrawListDto>
{
    public override void Apply(WarehouseWithdrawListDto packet) =>
        WhWithdrawListApplyIncomingPacketServices.ApplyWhWithdrawList(packet);
}
