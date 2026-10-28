using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowSeedInfo)]
public sealed class ExShowSeedInfoIncoming : IncomingWirePacket<ExShowSeedInfoDto>
{
    public override void Apply(ExShowSeedInfoDto packet) =>
        ManorIncomingPacketServices.ApplyExShowSeedInfo(packet);
}
