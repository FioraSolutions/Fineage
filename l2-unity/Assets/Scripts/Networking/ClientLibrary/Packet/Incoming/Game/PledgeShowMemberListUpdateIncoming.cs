using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeShowMemberListUpdate)]
public sealed class PledgeShowMemberListUpdateIncoming : IncomingWirePacket<PledgeShowMemberListUpdateDto>
{
    public override void Apply(PledgeShowMemberListUpdateDto packet)
    {
        PledgeIncomingPacketServices.ApplyPledgeShowMemberListUpdate(packet);
    }
}
