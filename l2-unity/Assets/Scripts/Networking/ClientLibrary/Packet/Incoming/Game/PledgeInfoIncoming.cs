
[IncomingGamePacket(GameServerPacketType.PledgeInfo)]
public sealed class PledgeInfoIncoming : IncomingWirePacket<PledgeInfoDto>
{
    public override void Apply(PledgeInfoDto packet) =>
        PledgeInfoIncomingPacketServices.ApplyPledgeInfo(packet);
}
