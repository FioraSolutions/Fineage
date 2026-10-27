using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowSeedInfo)]
public sealed class ExShowSeedInfoIncoming : IncomingWirePacket<ExShowSeedInfoDto>
{
    public override void Apply(ExShowSeedInfoDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.ShowWindowActiveTabSeed();
            WorldIncomingPacketServices.SeedInfo.SetDataSeedInfo(packet.List);
        });
    }
}
