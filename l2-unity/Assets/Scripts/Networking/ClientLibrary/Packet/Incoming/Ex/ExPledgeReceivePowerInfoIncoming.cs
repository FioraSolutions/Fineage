
[IncomingExPacket(GameExServerPacketType.ExPledgeReceivePowerInfo)]
public sealed class ExPledgeReceivePowerInfoIncoming : IncomingWirePacket<PledgeReceivePowerInfoDto>
{
    public override void Apply(PledgeReceivePowerInfoDto packet) =>
        ExPledgeReceivePowerInfoApplyIncomingPacketServices.ApplyExPledgeReceivePowerInfo(packet);
}
