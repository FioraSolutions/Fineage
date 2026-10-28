
[IncomingGamePacket(GameServerPacketType.MoveToPawn)]
public sealed class MoveToPawnIncoming : IncomingWirePacket<MoveToPawnDto>
{
    public override void Apply(MoveToPawnDto packet) =>
        MoveToPawnApplyIncomingPacketServices.ApplyPawn(packet);
}
