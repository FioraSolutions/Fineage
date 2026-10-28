using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowSellCropList)]
public sealed class ExShowSellCropListIncoming : IncomingWirePacket<ExShowSellCropListDto>
{
    public override void Apply(ExShowSellCropListDto packet) =>
        ManorIncomingPacketServices.ApplyExShowSellCropList(packet);
}
