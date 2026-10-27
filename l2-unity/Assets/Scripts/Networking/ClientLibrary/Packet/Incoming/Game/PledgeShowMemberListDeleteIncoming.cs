using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListDelete)]
public sealed class PledgeShowMemberListDeleteIncoming : IncomingWirePacket<PledgeShowMemberListDeleteDto>
{
    public override void Apply(PledgeShowMemberListDeleteDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.DeleteMemberData(packet));
    }
}
