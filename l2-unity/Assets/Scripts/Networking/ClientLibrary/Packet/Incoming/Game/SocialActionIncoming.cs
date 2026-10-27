[IncomingGamePacket(GameServerPacketType.SocialAction)]
public sealed class SocialActionIncoming : IncomingWirePacket<SocialActionDto>
{
    public override void Apply(SocialActionDto packet)
    {
        WorldIncomingPacketServices.ApplyWorld(apply => apply.SocialAction(packet));
    }
}
