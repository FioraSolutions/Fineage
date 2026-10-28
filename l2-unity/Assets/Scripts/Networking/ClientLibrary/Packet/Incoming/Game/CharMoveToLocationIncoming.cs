[IncomingGamePacket(GameServerPacketType.CharMoveToLocation)]
public sealed class CharMoveToLocationIncoming : IncomingWirePacket<CharMoveToLocationDto>
{
    public override void Apply(CharMoveToLocationDto packet) =>
        CharMoveToLocationIncomingPacketServices.ApplyMove(packet);
}
