[IncomingGamePacket(GameServerPacketType.CreatureSay)]
public sealed class CreatureSayIncoming : IncomingWirePacket<CreatureSayDto>
{
    public override void Apply(CreatureSayDto packet)
    {
        if (packet.Message == null)
            return;

        MessageIncomingPacketServices.ApplyCreatureSay(packet.Message);
    }
}
