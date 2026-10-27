using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListUpdate)]
public sealed class PledgeShowMemberListUpdateIncoming : IncomingWirePacket<PledgeShowMemberListUpdateDto>
{
    public override void Apply(PledgeShowMemberListUpdateDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Clan.UpdateMemberData(packet));
    }
}
