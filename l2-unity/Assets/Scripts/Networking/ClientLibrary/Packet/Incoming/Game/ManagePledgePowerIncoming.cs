using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ManagePledgePower)]
public sealed class ManagePledgePowerIncoming : IncomingWirePacket<ManagePledgePowerDto>
{
    public override void Apply(ManagePledgePowerDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));
    }
}
