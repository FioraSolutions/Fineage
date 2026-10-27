[IncomingGamePacket(GameServerPacketType.CharSelectionInfo)]
public sealed class CharSelectionInfoIncoming : IncomingWirePacket<CharSelectionInfoDto>
{
    public override void Apply(CharSelectionInfoDto packet)
    {
        CharacterIncomingPacketServices.ApplyCharSelectionInfo(packet);
    }
}
