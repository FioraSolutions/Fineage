[IncomingGamePacket(GameServerPacketType.CharCreateFail)]
public sealed class CharCreateFailIncoming : IncomingWirePacket<CharCreateFailDto>
{
    public override void Apply(CharCreateFailDto packet) =>
        CharCreateFailIncomingPacketServices.ApplyCharCreateFail(packet);
}
