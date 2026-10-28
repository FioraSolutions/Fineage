[IncomingGamePacket(GameServerPacketType.CharInfo)]
public sealed class CharInfoIncoming : IncomingWirePacket<CharInfoDto>
{
    public override void Apply(CharInfoDto packet) =>
        CharInfoIncomingPacketServices.ApplyCharInfo(packet);
}
