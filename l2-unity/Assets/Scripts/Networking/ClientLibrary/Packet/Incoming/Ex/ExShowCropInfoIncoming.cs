using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowCropInfo)]
public sealed class ExShowCropInfoIncoming : IncomingWirePacket<ExShowCropInfoDto>
{
    public override void Apply(ExShowCropInfoDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.SeedInfo.SetDataCropInfo(packet.List);
            IncomingPacketServices.SeedInfo.ShowWindow();
        });
    }
}
