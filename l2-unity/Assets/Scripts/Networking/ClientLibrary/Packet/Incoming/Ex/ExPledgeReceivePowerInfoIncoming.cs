using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExPledgeReceivePowerInfo)]
public sealed class ExPledgeReceivePowerInfoIncoming : IncomingWirePacket<PledgeReceivePowerInfoDto>
{
    public override void Apply(PledgeReceivePowerInfoDto packet) =>
        PledgeIncomingPacketServices.ApplyExPledgeReceivePowerInfo(packet);
}
