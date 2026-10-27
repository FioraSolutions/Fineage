using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExOnExPledgePowerGradeList)]
public sealed class ExPledgePowerGradeListIncoming : IncomingWirePacket<PledgePowerGradeListDto>
{
    public override void Apply(PledgePowerGradeListDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Clan.ShowGradeInfo(packet));
    }
}
