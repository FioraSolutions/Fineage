
[IncomingExPacket(GameExServerPacketType.ExPledgeReceiveMemberInfo)]
public sealed class ExPledgeReceiveMemberInfoIncoming : IncomingWirePacket<PledgeReceiveMemberInfoDto>
{
    public override void Apply(PledgeReceiveMemberInfoDto packet) =>
        PledgeIncomingPacketServices.ApplyExPledgeReceiveMemberInfo(packet);
}
