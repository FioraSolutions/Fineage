using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowSeedInfo)]
public sealed class ExShowSeedInfoIncoming : IncomingWirePacket<ExShowSeedInfoDto>
{
    public override void Apply(ExShowSeedInfoDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.SeedInfo.ShowWindowActiveTabSeed();
            IncomingPacketServices.SeedInfo.SetDataSeedInfo(packet.List);
        });
    }
}
