using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowSellCropList)]
public sealed class ExShowSellCropListIncoming : IncomingWirePacket<ExShowSellCropListDto>
{
    public override void Apply(ExShowSellCropListDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            WorldIncomingPacketServices.SellCrop.ShowWindow();
            WorldIncomingPacketServices.SellCrop.SetDataTable(packet);
        });
    }
}
