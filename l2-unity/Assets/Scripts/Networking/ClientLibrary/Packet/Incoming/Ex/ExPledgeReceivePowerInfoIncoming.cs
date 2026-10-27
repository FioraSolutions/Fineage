using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExPledgeReceivePowerInfo)]
public sealed class ExPledgeReceivePowerInfoIncoming : IncomingWirePacket<PledgeReceivePowerInfoDto>
{
    public override void Apply(PledgeReceivePowerInfoDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Clan.UpdateDetailedInfo(packet));
    }
}
