
[IncomingExPacket(GameExServerPacketType.ExShowSellCropList)]
public sealed class ExShowSellCropListIncoming : IncomingWirePacket<ExShowSellCropListDto>
{
    public override void Apply(ExShowSellCropListDto packet) =>
        ExShowSellCropListIncomingPacketServices.ApplyExShowSellCropList(packet);
}
