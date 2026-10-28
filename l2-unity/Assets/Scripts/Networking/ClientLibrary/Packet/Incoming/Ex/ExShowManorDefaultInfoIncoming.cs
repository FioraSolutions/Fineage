
[IncomingExPacket(GameExServerPacketType.ExShowManorDefaultInfo)]
public sealed class ExShowManorDefaultInfoIncoming : IncomingWirePacket<ExShowManorDefaultInfoDto>
{
    public override void Apply(ExShowManorDefaultInfoDto packet) =>
        ExShowManorDefaultInfoIncomingPacketServices.ApplyExShowManorDefaultInfo(packet);
}
