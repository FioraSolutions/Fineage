using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeInfo)]
public sealed class PledgeInfoIncoming : IncomingWirePacket<PledgeInfoDto>
{
    public override void Apply(PledgeInfoDto packet) =>
        PledgeIncomingPacketServices.ApplyPledgeInfo(packet);
}
