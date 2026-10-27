using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListDelete)]
public sealed class PledgeShowMemberListDeleteIncoming : IncomingWirePacket<PledgeShowMemberListDeleteDto>
{
    public override void Apply(PledgeShowMemberListDeleteDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Clan.DeleteMemberData(packet));
    }
}
