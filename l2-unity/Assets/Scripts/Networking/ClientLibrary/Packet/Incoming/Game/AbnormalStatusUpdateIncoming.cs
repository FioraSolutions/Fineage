[IncomingGamePacket(GameServerPacketType.AbnormalStatusUpdate)]
public sealed class AbnormalStatusUpdateIncoming : IncomingWirePacket<AbnormalStatusUpdateDto>
{
    public override void Apply(AbnormalStatusUpdateDto packet)
    {
        EntityAnimIncomingPacketServices.ApplyAbnormalStatusUpdate(packet);
    }
}
