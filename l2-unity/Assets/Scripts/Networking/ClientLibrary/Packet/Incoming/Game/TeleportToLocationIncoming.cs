[IncomingGamePacket(GameServerPacketType.TeleportToLocation)]
public sealed class TeleportToLocationIncoming : IncomingWirePacket<TeleportToLocationDto>
{
    public override void Apply(TeleportToLocationDto packet) =>
        TeleportToLocationApplyIncomingPacketServices.ApplyTeleportToLocation(packet);
}
