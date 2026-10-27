using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExOnExPledgePowerGradeList)]
public sealed class ExPledgePowerGradeListIncoming : IncomingWirePacket<PledgePowerGradeListDto>
{
    public override void Apply(PledgePowerGradeListDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.ShowGradeInfo(packet));
    }
}
