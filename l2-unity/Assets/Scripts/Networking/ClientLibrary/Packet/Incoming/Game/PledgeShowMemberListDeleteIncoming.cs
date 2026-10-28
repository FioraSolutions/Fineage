
[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListDelete)]
public sealed class PledgeShowMemberListDeleteIncoming : IncomingWirePacket<PledgeShowMemberListDeleteDto>
{
    public override void Apply(PledgeShowMemberListDeleteDto packet) =>
        PledgeShowMemberListDeleteIncomingPacketServices.ApplyPledgeShowMemberListDelete(packet);
}
