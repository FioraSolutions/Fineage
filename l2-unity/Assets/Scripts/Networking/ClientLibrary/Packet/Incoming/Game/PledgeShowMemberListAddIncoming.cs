
[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListAdd)]
public sealed class PledgeShowMemberListAddIncoming : IncomingWirePacket<PledgeShowMemberListAddDto>
{
    public override void Apply(PledgeShowMemberListAddDto packet) =>
        PledgeShowMemberListAddApplyIncomingPacketServices.ApplyPledgeShowMemberListAdd(packet);
}
