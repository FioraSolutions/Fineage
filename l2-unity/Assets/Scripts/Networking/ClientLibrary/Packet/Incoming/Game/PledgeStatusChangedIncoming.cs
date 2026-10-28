
[IncomingGamePacket(GameServerPacketType.PledgeStatusChanged)]
public sealed class PledgeStatusChangedIncoming : IncomingWirePacket<PledgeStatusChangedDto>
{
    public override void Apply(PledgeStatusChangedDto packet) =>
        PledgeStatusChangedIncomingPacketServices.ApplyPledgeStatusChanged(packet);
}
