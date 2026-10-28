
[IncomingGamePacket(GameServerPacketType.EtcStatusUpdate)]
public sealed class EtcStatusUpdateIncoming : IncomingWirePacket<EtcStatusUpdateDto>
{
    public override void Apply(EtcStatusUpdateDto packet) =>
        EtcStatusUpdateIncomingPacketServices.ApplyEtcStatusUpdate(packet);
}
