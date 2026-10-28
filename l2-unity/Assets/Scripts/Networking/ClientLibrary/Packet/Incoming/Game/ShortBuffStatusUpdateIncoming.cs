
[IncomingGamePacket(GameServerPacketType.ShortBuffStatusUpdate)]
public sealed class ShortBuffStatusUpdateIncoming : IncomingWirePacket<ShortBuffStatusUpdateDto>
{
    public override void Apply(ShortBuffStatusUpdateDto packet) =>
        ShortBuffStatusUpdateApplyIncomingPacketServices.ApplyShortBuffStatusUpdate(packet);
}
