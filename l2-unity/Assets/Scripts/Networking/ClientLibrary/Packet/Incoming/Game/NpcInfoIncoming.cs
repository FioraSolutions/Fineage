
[IncomingGamePacket(GameServerPacketType.NpcInfo)]
public sealed class NpcInfoIncoming : IncomingWirePacket<NpcInfoDto>
{
    public override void Apply(NpcInfoDto packet) =>
        NpcInfoApplyIncomingPacketServices.ApplyNpcInfo(packet);
}
