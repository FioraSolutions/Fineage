[IncomingGamePacket(GameServerPacketType.SocialAction)]
public sealed class SocialActionIncoming : IncomingWirePacket<SocialActionDto>
{
    public override void Apply(SocialActionDto packet)
    {
        EntityAnimIncomingPacketServices.ApplySocialAction(packet);
    }
}
