[IncomingGamePacket(GameServerPacketType.InterludeKeyPacket)]
public sealed class InterludeKeyIncoming : IncomingWirePacket<InterludeKeyDto>
{
    protected override void OnParsed(InterludeKeyDto packet) =>
        InterludeKeyIncomingPacketServices.OnInterludeKeyParsed(packet);

    public override void Apply(InterludeKeyDto packet) =>
        InterludeKeyIncomingPacketServices.ApplyInterludeKey(packet);
}
