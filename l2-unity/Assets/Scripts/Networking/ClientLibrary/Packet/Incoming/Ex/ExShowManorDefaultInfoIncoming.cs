using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowManorDefaultInfo)]
public sealed class ExShowManorDefaultInfoIncoming : IncomingWirePacket<ExShowManorDefaultInfoDto>
{
    public override void Apply(ExShowManorDefaultInfoDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.SeedInfo.SetDataDefaultManorInfo(packet.List);
            IncomingPacketServices.SeedInfo.ShowWindowActiveTabAllDefault();
        });
    }
}
