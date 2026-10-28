
[IncomingExPacket(GameExServerPacketType.ExShowSellCropList)]
public sealed class ExShowSellCropListIncoming : IncomingWirePacket<ExShowSellCropListDto>
{
    public override void Apply(ExShowSellCropListDto packet) =>
        ExShowSellCropListApplyIncomingPacketServices.ApplyExShowSellCropList(packet);
}
