using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListAll)]
public sealed class PledgeShowMemberListAllIncoming : IncomingWirePacket<PledgeShowMemberListAllDto>
{
    public override void Apply(PledgeShowMemberListAllDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.AddClanData(packet));
    }
}
