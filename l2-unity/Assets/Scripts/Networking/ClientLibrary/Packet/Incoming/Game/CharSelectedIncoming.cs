[IncomingGamePacket(GameServerPacketType.CharSelected)]
public sealed class CharSelectedIncoming : IncomingWirePacket<CharSelectedDto>
{
    protected override void OnParsed(CharSelectedDto packet)
    {
        CharacterIncomingPacketServices.OnCharSelectedParsed(packet);
    }

    public override void Apply(CharSelectedDto packet)
    {
        CharacterIncomingPacketServices.ApplyCharSelected(packet);
    }
}
