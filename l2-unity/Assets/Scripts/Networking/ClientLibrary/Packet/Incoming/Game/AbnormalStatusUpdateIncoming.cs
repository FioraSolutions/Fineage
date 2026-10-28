[IncomingGamePacket(GameServerPacketType.AbnormalStatusUpdate)]
public sealed class AbnormalStatusUpdateIncoming : IncomingWirePacket<AbnormalStatusUpdateDto>
{
    public override void Apply(AbnormalStatusUpdateDto packet) =>
        AbnormalStatusUpdateIncomingPacketServices.ApplyAbnormalStatusUpdate(packet);
}
