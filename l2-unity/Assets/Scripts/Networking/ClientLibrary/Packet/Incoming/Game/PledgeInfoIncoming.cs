using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeInfo)]
public sealed class PledgeInfoIncoming : IncomingWirePacket<PledgeInfoDto>
{
    public override void Apply(PledgeInfoDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Clan.UpdatePledge(packet));
    }
}
