using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PledgeStatusChanged)]
public sealed class PledgeStatusChangedIncoming : IncomingWirePacket<PledgeStatusChangedDto>
{
    public override void Apply(PledgeStatusChangedDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.UpdateClanIdInfo(packet));
    }
}
