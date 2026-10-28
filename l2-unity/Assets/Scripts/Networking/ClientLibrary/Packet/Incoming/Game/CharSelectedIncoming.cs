[IncomingGamePacket(GameServerPacketType.CharSelected)]
public sealed class CharSelectedIncoming : IncomingWirePacket<CharSelectedDto>
{
    protected override void OnParsed(CharSelectedDto packet) =>
        CharSelectedIncomingPacketServices.OnCharSelectedParsed(packet);

    public override void Apply(CharSelectedDto packet) =>
        CharSelectedIncomingPacketServices.ApplyCharSelected(packet);
}
