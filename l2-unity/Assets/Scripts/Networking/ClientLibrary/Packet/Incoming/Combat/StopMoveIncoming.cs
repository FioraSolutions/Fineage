
[IncomingGamePacket(GameServerPacketType.StopMove)]
public sealed class StopMoveIncoming : IncomingWirePacket<StopMoveDto>
{
    public override void Apply(StopMoveDto packet) =>
        StopMoveIncomingPacketServices.ApplyStop(packet);
}
