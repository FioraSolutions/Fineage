using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListUpdate)]
public sealed class PledgeShowMemberListUpdateIncoming : IncomingWirePacket<PledgeShowMemberListUpdateDto>
{
    public override void Apply(PledgeShowMemberListUpdateDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.UpdateMemberData(packet));
    }
}
