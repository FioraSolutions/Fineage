
[IncomingExPacket(GameExServerPacketType.ExShowManorDefaultInfo)]
public sealed class ExShowManorDefaultInfoIncoming : IncomingWirePacket<ExShowManorDefaultInfoDto>
{
    public override void Apply(ExShowManorDefaultInfoDto packet) =>
        ManorIncomingPacketServices.ApplyExShowManorDefaultInfo(packet);
}
