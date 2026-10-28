
[IncomingGamePacket(GameServerPacketType.CharCreateOk)]
public sealed class CharCreateOkIncoming : IncomingWirePacket<CharCreateOkDto>
{
    public override void Apply(CharCreateOkDto packet) =>
        CharCreateOkIncomingPacketServices.ApplyCharCreateOk(packet);
}
