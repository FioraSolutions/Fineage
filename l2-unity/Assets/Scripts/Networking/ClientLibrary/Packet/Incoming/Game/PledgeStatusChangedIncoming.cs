using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeStatusChanged)]
public sealed class PledgeStatusChangedIncoming : IncomingWirePacket<PledgeStatusChangedDto>
{
    public override void Apply(PledgeStatusChangedDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Clan.UpdateClanIdInfo(packet));
    }
}
