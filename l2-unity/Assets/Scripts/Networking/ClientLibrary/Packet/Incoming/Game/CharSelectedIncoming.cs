[IncomingGamePacket(GameServerPacketType.CharSelected)]
public sealed class CharSelectedIncoming : IncomingWirePacket<CharSelectedDto>
{
    protected override void OnParsed(CharSelectedDto packet) =>
        CharSelectedParsedApplyIncomingPacketServices.OnCharSelectedParsed(packet);

    public override void Apply(CharSelectedDto packet) =>
        CharSelectedApplyIncomingPacketServices.ApplyCharSelected(packet);
}
