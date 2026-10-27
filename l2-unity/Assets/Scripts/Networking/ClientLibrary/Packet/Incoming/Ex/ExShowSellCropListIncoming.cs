using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowSellCropList)]
public sealed class ExShowSellCropListIncoming : IncomingWirePacket<ExShowSellCropListDto>
{
    public override void Apply(ExShowSellCropListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.SellCrop.ShowWindow();
            IncomingPacketServices.SellCrop.SetDataTable(packet);
        });
    }
}
