
[IncomingGamePacket(GameServerPacketType.HennaInfo)]
public sealed class HennaInfoIncoming : IncomingWirePacket<HennaInfoDto>
{
    public override void Apply(HennaInfoDto packet) =>
        HennaInfoApplyIncomingPacketServices.ApplyHennaInfo(packet);
}
