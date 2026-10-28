
[IncomingExPacket(GameExServerPacketType.ExOnExPledgePowerGradeList)]
public sealed class ExPledgePowerGradeListIncoming : IncomingWirePacket<PledgePowerGradeListDto>
{
    public override void Apply(PledgePowerGradeListDto packet) =>
        ExPledgePowerGradeListApplyIncomingPacketServices.ApplyExPledgePowerGradeList(packet);
}
