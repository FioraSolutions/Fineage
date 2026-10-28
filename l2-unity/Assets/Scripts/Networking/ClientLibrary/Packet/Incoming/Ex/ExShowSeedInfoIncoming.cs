
[IncomingExPacket(GameExServerPacketType.ExShowSeedInfo)]
public sealed class ExShowSeedInfoIncoming : IncomingWirePacket<ExShowSeedInfoDto>
{
    public override void Apply(ExShowSeedInfoDto packet) =>
        ExShowSeedInfoApplyIncomingPacketServices.ApplyExShowSeedInfo(packet);
}
