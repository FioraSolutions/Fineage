
[IncomingExPacket(GameExServerPacketType.ExShowCropInfo)]
public sealed class ExShowCropInfoIncoming : IncomingWirePacket<ExShowCropInfoDto>
{
    public override void Apply(ExShowCropInfoDto packet) =>
        ManorIncomingPacketServices.ApplyExShowCropInfo(packet);
}
