
[IncomingExPacket(GameExServerPacketType.ExShowSeedInfo)]
public sealed class ExShowSeedInfoIncoming : IncomingWirePacket<ExShowSeedInfoDto>
{
    public override void Apply(ExShowSeedInfoDto packet) =>
        ExShowSeedInfoIncomingPacketServices.ApplyExShowSeedInfo(packet);
}
