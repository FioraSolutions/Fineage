[IncomingGamePacket(GameServerPacketType.InterludeKeyPacket)]
public sealed class InterludeKeyIncoming : IncomingWirePacket<InterludeKeyDto>
{
    protected override void OnParsed(InterludeKeyDto packet) =>
        LobbyWireIncomingPacketServices.OnInterludeKeyParsed(packet);

    public override void Apply(InterludeKeyDto packet) =>
        LobbyWireIncomingPacketServices.ApplyInterludeKey(packet);
}
