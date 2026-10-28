
[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListAll)]
public sealed class PledgeShowMemberListAllIncoming : IncomingWirePacket<PledgeShowMemberListAllDto>
{
    public override void Apply(PledgeShowMemberListAllDto packet) =>
        PledgeShowMemberListAllApplyIncomingPacketServices.ApplyPledgeShowMemberListAll(packet);
}
