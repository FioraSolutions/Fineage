using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExPledgeReceiveMemberInfo)]
public sealed class ExPledgeReceiveMemberInfoIncoming : IncomingWirePacket<PledgeReceiveMemberInfoDto>
{
    public override void Apply(PledgeReceiveMemberInfoDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));
    }
}
