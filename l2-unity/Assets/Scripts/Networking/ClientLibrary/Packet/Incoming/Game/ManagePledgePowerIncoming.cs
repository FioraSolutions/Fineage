
[IncomingGamePacket(GameServerPacketType.ManagePledgePower)]
public sealed class ManagePledgePowerIncoming : IncomingWirePacket<ManagePledgePowerDto>
{
    public override void Apply(ManagePledgePowerDto packet) =>
        ManagePledgePowerIncomingPacketServices.ApplyManagePledgePower(packet);
}
