using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListAdd)]
public sealed class PledgeShowMemberListAddIncoming : IncomingWirePacket<PledgeShowMemberListAddDto>
{
    public override void Apply(PledgeShowMemberListAddDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.AddMemberData(packet));
    }
}
