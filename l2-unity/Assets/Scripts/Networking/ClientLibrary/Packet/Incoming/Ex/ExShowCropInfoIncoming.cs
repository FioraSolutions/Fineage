using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowCropInfo)]
public sealed class ExShowCropInfoIncoming : IncomingWirePacket<ExShowCropInfoDto>
{
    public override void Apply(ExShowCropInfoDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.SetDataCropInfo(packet.List);
            WorldIncomingPacketServices.SeedInfo.ShowWindow();
        });
    }
}
