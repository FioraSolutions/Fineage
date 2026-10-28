
[IncomingExPacket(GameExServerPacketType.ExShowCropInfo)]
public sealed class ExShowCropInfoIncoming : IncomingWirePacket<ExShowCropInfoDto>
{
    public override void Apply(ExShowCropInfoDto packet) =>
        ExShowCropInfoIncomingPacketServices.ApplyExShowCropInfo(packet);
}
