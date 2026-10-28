[IncomingGamePacket(GameServerPacketType.CharCreateFail)]
public sealed class CharCreateFailIncoming : IncomingWirePacket<CharCreateFailDto>
{
    public override void Apply(CharCreateFailDto packet) =>
        CharCreateFailApplyIncomingPacketServices.ApplyCharCreateFail(packet);
}
