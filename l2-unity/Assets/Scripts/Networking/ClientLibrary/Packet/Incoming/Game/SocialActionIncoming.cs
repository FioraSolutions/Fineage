[IncomingGamePacket(GameServerPacketType.SocialAction)]
public sealed class SocialActionIncoming : IncomingWirePacket<SocialActionDto>
{
    public override void Apply(SocialActionDto packet)
    {
        IncomingPacketServices.ApplyWorld(apply => apply.SocialAction(packet));
    }
}
