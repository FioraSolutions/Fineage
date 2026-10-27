using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowManorDefaultInfo)]
public sealed class ExShowManorDefaultInfoIncoming : IncomingWirePacket<ExShowManorDefaultInfoDto>
{
    public override void Apply(ExShowManorDefaultInfoDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.SetDataDefaultManorInfo(packet.List);
            WorldIncomingPacketServices.SeedInfo.ShowWindowActiveTabAllDefault();
        });
    }
}
